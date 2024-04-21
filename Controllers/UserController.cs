using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Cinder.Models;
using Cinder.Data;
using Cinder.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Cinder.Controllers {
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationContext _context;

            public UserController(UserManager<User> userManager, ApplicationContext context)
            {
                _userManager = userManager;
                _context = context;
            }

        
        /// <summary>
        /// Displays the user profile completion form.
        /// </summary>
        /// <param name="userId">The ID of the user whose profile is to be completed.</param>
        /// <returns>A view with form to complete user profile.</returns>
        public async Task<IActionResult> CompleteProfile(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Fetch matched users
            var matchedUsers = await _context.Matches
                .Where(m => m.Id_User1 == userId || m.Id_User2 == userId)
                .Include(m => m.User2) // Include the related user details
                .ToListAsync();

            // Pass both the user and matched users to the view using ViewBag
            ViewBag.User = user;
            ViewBag.MatchedUsers = matchedUsers;

            ViewBag.Languages = new SelectList(_context.Languages, "Id_Language", "Name");
            ViewBag.Faculties = new SelectList(_context.Faculties, "Id_Faculty", "Name");
            ViewBag.Hobbies = new SelectList(_context.Hobbies, "Id_Hobby", "Name");

            return View(user);
        }
        
        /// <summary>
        /// Handles the submission of the profile completion form.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <param name="user">The user details to update.</param>
        /// <returns>Redirects to the appropriate next step based on user role and form completion.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteProfile (string id, [Bind("Id,Bio,Age,Id_Faculty,FacultyYear,Sex,Employed,Employment,Smoker,Pets,Id_Language,LeaseDuration,Id_Hobby")] User user){
            if (id != user.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Debug.WriteLine(error.ErrorMessage);
                    }
                }
            }

            if (ModelState.IsValid)
                {
                    try
                    {
                        var existingUser = await _userManager.FindByIdAsync(id);
                        existingUser.Bio = user.Bio;
                        existingUser.Age = user.Age;
                        existingUser.Id_Faculty = user.Id_Faculty;
                        existingUser.Faculty= _context.Faculties.Find(user.Id_Faculty);
                        existingUser.FacultyYear = user.FacultyYear;
                        existingUser.Sex=user.Sex;
                        existingUser.Employed=user.Employed;
                        existingUser.Employment=user.Employment;
                        existingUser.Smoker=user.Smoker;
                        existingUser.Pets=user.Pets;
                        if(User.IsInRole("Taker")){
                            existingUser.MyBooleanProperty = true;
                        }
                        existingUser.LeaseDuration = user.LeaseDuration;

                        _context.Update(existingUser);


                        var selectedLanguages = _context.Languages.Where(l => user.Id_Language.Contains(l.Id_Language)).ToList();
                        foreach (var languageId in selectedLanguages)
                        {
                            var userLanguage = new UserLanguage
                            {
                                UserId = existingUser.Id,
                                LanguageId = languageId.Id_Language
                            };
                            _context.UserLanguages.Add(userLanguage);
                        }

                        var selectedHobbies = _context.Hobbies.Where(h => user.Id_Hobby.Contains(h.Id_Hobby)).ToList();
                        foreach (var hobbyId in selectedHobbies)
                        {
                            var userHobby = new UserHobby
                            {
                                UserId = existingUser.Id,
                                HobbyId = hobbyId.Id_Hobby
                            };
                            _context.UserHobbies.Add(userHobby);
                        }
                        await _context.SaveChangesAsync();

                        var users = await _context.Users.Where(u => u.Id != existingUser.Id)
                                                        .ToListAsync();

                        var currentUserRole = (await _userManager.GetRolesAsync(existingUser)).FirstOrDefault();

                        var existingUserLanguages = await _context.UserLanguages
                            .Where(ul => ul.UserId == existingUser.Id)
                            .Select(ul => ul.LanguageId)
                            .ToListAsync();

                        var existingUserHobbies = await _context.UserHobbies
                            .Where(uh => uh.UserId == existingUser.Id)
                            .Select(uh => uh.HobbyId)
                            .ToListAsync();

                        foreach (var exUser in users)
                        {
                            if (exUser.MyBooleanProperty == true)  {
                                var existingUserRole = (await _userManager.GetRolesAsync(exUser)).FirstOrDefault();
                                if (currentUserRole != existingUserRole)
                                {
                                    //calculate points
                                    var exUserLanguages = await _context.UserLanguages
                                        .Where(ul => ul.UserId == exUser.Id)
                                        .Select(ul => ul.LanguageId)
                                        .ToListAsync();
                                    var commonLanguages = existingUserLanguages.Intersect(exUserLanguages).Count();
                                    var exUserHobbies = await _context.UserHobbies
                                        .Where(uh => uh.UserId == exUser.Id)
                                        .Select(uh => uh.HobbyId)
                                        .ToListAsync();
                                    var commonHobbies = existingUserHobbies.Intersect(exUserHobbies).Count();
                                    int points = 0;

                                    if (existingUser.Id_Faculty == exUser.Id_Faculty)
                                        points += 7;
                                    points += commonLanguages * 5;
                                    points += commonHobbies * 3;

                                    var user1=existingUser;
                                    var user2=exUser;

                                    
                                    //add users to the table
                                    var match1 = new Match { Id_User1 = existingUser.Id, Id_User2 = exUser.Id, points = points, User1 = user1, User2 = user2 };
                                    _context.Matches.Add(match1);

                                    var match2 = new Match { Id_User2 = existingUser.Id, Id_User1 = exUser.Id, points = points, User1 = user2, User2 = user1  };
                                    _context.Matches.Add(match2);
                                }
                            }
                        }


                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        
                    }
                    if (User.IsInRole("Giver"))
                    {
                        return RedirectToAction("MakeProperty", "Property", new { userId = user.Id });
                    }else{
                        return RedirectToAction("Index", "Home");
                    }
                }
                return View(user);

        }

        /// <summary>
        /// Displays the matched users based on the current user's matches.
        /// </summary>
        /// <returns>A view listing matched users.</returns>
        [Authorize]
        public async Task<IActionResult> MatchedUsers()
        {
            var userId = _userManager.GetUserId(User); // Get the current logged-in user's ID

            // Fetch matches where the current user is involved
            var matchedUsers = await _context.Matches
                .Where(m => m.Id_User1 == userId || m.Id_User2 == userId)
                .Include(m => m.User2) // Include the related user details
                .ToListAsync();

            return View(matchedUsers);
        }}
}