using Microsoft.AspNetCore.Mvc;
using Cinder.Models;
using Cinder.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Cinder.Controllers;

[Authorize]
public class PropertyController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly ApplicationContext _context;
    private readonly UserService _userService;

    private readonly HttpClient _httpClient = new HttpClient();

    public PropertyController(UserManager<User> userManager, ApplicationContext context, UserService userService)
{
    _userManager = userManager;
    _context = context;
    _userService = userService;
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
                property.UserId = UserId;
                _context.Add(property);
                await _context.SaveChangesAsync();

                _context.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("AddRoom", "Property", new { propertyId = property.Id_Property });
            }
            else
            {
                return NotFound("User not found");
            }
        }
        ViewBag.UserId = UserId;
        return View(property);
    }

    // GET: Property/AddRoom/5
    public IActionResult AddRoom(int? propertyId)
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
    public async Task<IActionResult> AddRoom(int propertyId, string addAnother, [Bind("Type, Description, Image, Price, Utilities, MoveInDate, MoveOutDate, Furnishings, SquareMeters, Heating, Cooling, PrivateBathroom, PrivateKitchen, PrivateBalcony, PrivateTerrace")] Room room)
    {
        if (ModelState.IsValid)
        {
            var property = await _context.Properties.FindAsync(propertyId);
            if (property != null)
            {
                room.Property = property;
                property.Rooms.Add(room);
                room.Id_Property = propertyId;
                _context.Add(room);
                await _context.SaveChangesAsync();

                if (addAnother == "true")
                {
                    return RedirectToAction("AddRoom", "Property", new { propertyId = property.Id_Property });
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return NotFound("Property not found");
            }
        }
        ViewBag.PropertyId = propertyId;
        return View(room);
    }

    private async Task GenerateMatchesForUser(User user)
    {
        string existingUserId = user.Id;

        var allUsers = await _userService.GetAllUserData();
        var userData = new
        {
            ExistingUserId = existingUserId,
            Users = allUsers
        };

        var settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        var allUsersJson = JsonConvert.SerializeObject(userData, settings);

        var apiUrl = "http://localhost:5000/recommend";
        var content = new StringContent(allUsersJson, Encoding.UTF8, "application/json");

        HttpResponseMessage response = null;
        try
        {
            response = await _httpClient.PostAsync(apiUrl, content);
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var recommendations = JsonConvert.DeserializeObject<dynamic>(responseJson);

                foreach (var recommendation in recommendations.data)
                {
                    string recommendedUserId = recommendation[0];
                    double similarityScore = recommendation[1];

                    var match1 = new Match
                    {
                        Id_User1 = user.Id,
                        Id_User2 = recommendedUserId,
                        points = similarityScore,
                        User1 = user,
                        User2 = await _context.Users.FindAsync(recommendedUserId)
                    };
                    _context.Matches.Add(match1);

                    var match2 = new Match
                    {
                        Id_User2 = user.Id,
                        Id_User1 = recommendedUserId,
                        points = similarityScore,
                        User1 = await _context.Users.FindAsync(recommendedUserId),
                        User2 = user
                    };
                    _context.Matches.Add(match2);
                }

                await _context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("Flask API call failed with status code {0}", response.StatusCode);
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine("Error sending data to the Flask API: {0}", ex.Message);
        }
    }

    // Implement additional methods as needed
}
