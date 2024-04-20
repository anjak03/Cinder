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
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var potentialMatches = _context.Matches
            .Include(m => m.User1)
            .ThenInclude(u => u.MatchedUsers)
            .Include(m => m.User2)
            .Where(m => m.Id_User1 == userId && m.Matched == true)
            .ToList();

        return View(potentialMatches);
    }

}