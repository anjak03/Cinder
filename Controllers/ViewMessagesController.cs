using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Cinder.Models;
using Cinder.Data;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Cinder.Controllers;

public class ViewMessagesController : Controller {

    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationContext _context;

    public ViewMessagesController(ILogger<HomeController> logger, ApplicationContext context)
    {
        _logger = logger;
        _context = context;
        
    }

    public async Task<IActionResult> Messages()
    {
        // // Retrieve the ID of the logged-in user
        // var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // if (string.IsNullOrEmpty(userId))
        // {
        //     _logger.LogError("User is not logged in.");
        //     return Unauthorized();
        // }

        // // Fetch the user's details from the database
        // var user = await _context.Users.Include(u => u.MatchedUsers).
        //                          .FirstOrDefaultAsync(u => u.Id == userId);
        // if (user == null)
        // {
        //     _logger.LogError($"User not found with ID: {userId}");
        //     return NotFound();
        // }

        // return View("Messages", user); // Pass the user to the view
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        // Retrieve potential matches for the given user
        var potentialMatches = _context.Matches
            .Include(m => m.User1)
            .ThenInclude(u => u.MatchedUsers)
            .Include(m => m.User2)
            .Where(m => m.Id_User1 == userId && m.Matched == true)
            .ToList();

        return View(potentialMatches);
    }

}