using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Cinder.Models;
using Cinder.Data;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Cinder.Controllers {
    /// <summary>
    /// Controller responsible for handling user matchmaking functionality.
    /// </summary>
    public class MatchController : Controller {

        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationContext _context;

        public MatchController(ILogger<HomeController> logger, ApplicationContext context)
        {
            _logger = logger;
            _context = context;
            
        }

        /// <summary>
        /// Displays the list of potential matches for the current user.
        /// </summary>
        /// <returns>A view displaying potential matches.</returns>        
        public IActionResult Match()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var potentialMatches = _context.Matches
                .Include(m => m.User1)
                .Include(m => m.User2)
                .Include(m => m.User2.Property)
                .Include(m => m.User2.UserLanguages)
                .Include(m => m.User2.UserHobbies)
                .Include(m => m.User2.Faculty)
                .Include(m => m.User2.Property)
                    .ThenInclude(p => p.Rooms)
                .Where(m => m.Id_User1 == userId && m.Seen == false)
                .OrderByDescending(m => m.points)
                .ToList();

            return View(potentialMatches);
        }

                /// <summary>
        /// Processes the user's decision on a match, either approving or rejecting it.
        /// </summary>
        /// <param name="matchId">The ID of the match to update.</param>
        /// <param name="decision">The decision on the match (true for accept, false for reject).</param>
        /// <param name="userId1">The ID of the first user in the match.</param>
        /// <param name="userId2">The ID of the second user in the match.</param>
        /// <returns>A redirection to the updated list of matches.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Match(int matchId, bool decision, string userId1, string userId2){
            var match = await _context.Matches.FindAsync(matchId);
            var user1 = await _context.Users.FindAsync(userId1);
            var user2 = await _context.Users.FindAsync(userId2);

            if (match == null || user1 == null || user2 == null)
        {
            return NotFound();
        }
            if (decision)
            {
                match.User1_Swipe = 1;
                match.Seen = true;
                _context.Update(match);
                await _context.SaveChangesAsync();
                
                var reverseMatch = await _context.Matches.FirstOrDefaultAsync(m => m.Id_User1 == userId2 && m.Id_User2 == userId1);
                if (reverseMatch != null)
                {
                    reverseMatch.User2_Swipe = 1;
                    _context.Update(reverseMatch);
                    await _context.SaveChangesAsync();
                }

                if (match.User1_Swipe == 1 && match.User2_Swipe == 1)
                {
                    match.Matched = true;
                    _context.Update(match);

                    reverseMatch.Matched = true;
                    _context.Update(reverseMatch);

                    user1.MatchedUsers.Add(user2);
                    user2.MatchedUsers.Add(user1);
                    _context.Update(user1);
                    _context.Update(user2);
                    await _context.SaveChangesAsync();
                }
                
        
                return RedirectToAction(nameof(Match), new { currentIndex = 0});
        
            }
            else
            {
                match.User1_Swipe = 0;
                match.Seen = true;
                _context.Update(match);
                await _context.SaveChangesAsync();

                var reverseMatch = await _context.Matches.FirstOrDefaultAsync(m => m.Id_User1 == userId2 && m.Id_User2 == userId1);
                if (reverseMatch != null)
                {
                    reverseMatch.User2_Swipe = 0;
                    _context.Update(reverseMatch);
                    await _context.SaveChangesAsync();
                }

                
                return RedirectToAction(nameof(Match), new { currentIndex = 0});
            }
        }
    }
}