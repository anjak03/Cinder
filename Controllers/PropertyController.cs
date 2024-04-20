using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Cinder.Models;
using Cinder.Data;
using Cinder.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Cinder.Controllers;

public class PropertyController : Controller {
    private readonly UserManager<User> _userManager;
    private readonly ApplicationContext _context;

    public PropertyController(UserManager<User> userManager, ApplicationContext context) {
        _userManager = userManager;
        _context = context;
    }
    // GET: Property/MakeProperty
    public IActionResult MakeProperty(string userId)
    {
        ViewBag.UserId = userId;
        return View();
    }

    // POST: Property/MakeProperty
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MakeProperty(string UserId, [Bind("Type, Address, City, Neighborhood, SquareMeters, Description, Image, Deposit, Furnishings, Parking, PetsAllowed, SmokingAllowed, GuestsAllowed, Wifi, WashingMachine, ClosestPublicTransport, ClosestGorceryStore, HouseRules, NumberOfBathrooms, NumberOfBedrooms, MaxNumberOfTenants")] Property property)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user != null)
            {
                user.Property = property;
                property.UserId= UserId;
                _context.Add(property);
                await _context.SaveChangesAsync();

                _context.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("AddRoom", "Property", new { propertyId = property.Id_Property });
            }
            else
            {
                // Handle the case where the user is not found
                return NotFound("User not found");
            }
        }

        ViewBag.UserId = UserId;

        return View(property);
    }

    // GET: Property/AddRoom/5
    public async Task<IActionResult> AddRoom(int? propertyId)
    {
        if (propertyId == null)
        {
            return NotFound();
        }

        ViewBag.PropertyId = propertyId;
        return View();
    }

    // POST: Property/AddRoom/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddRoom(int Id_Property, string addAnother, [Bind("Type, Description, Image, Price, Utilities, MoveInDate, MoveOutDate, Furnishings, SquareMeters, Heating, Cooling, PrivateBathroom, PrivateKitchen, PrivateBalcony, PrivateTerrace")] Room room)
    {
        if (ModelState.IsValid)
        {
            var property = await _context.Properties.FindAsync(Id_Property);
            if (property != null)
            {
                room.Property = property;
                property.Rooms.Add(room);
                room.Id_Property = Id_Property;
                _context.Add(room);
                await _context.SaveChangesAsync();

                _context.Update(property);
                await _context.SaveChangesAsync();
                if (addAnother == "true")
                {
                    return RedirectToAction("AddRoom", "Property", new { propertyId = property.Id_Property });
                }
                else
                {
                    var user = await _userManager.FindByIdAsync(property.UserId);
                    user.MyBooleanProperty = true;
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                // Handle the case where the user is not found
                return NotFound("Property not found");
            }
        }
        // If ModelState is not valid, print validation errors
        foreach (var modelState in ModelState.Values)
        {
            foreach (var error in modelState.Errors)
            {
                // Print or log the validation error
                Console.WriteLine($"Validation Error: {error.ErrorMessage}");
            }
        }
        ViewBag.PropertyId = Id_Property;

        return View(room);
    }
}