using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Cinder.Models;
using Cinder.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Cinder.Dtos;

namespace Cinder.Controllers;

public class UserController : Controller
{
    private readonly UserService _userService;

    private readonly HttpClient _httpClient = new HttpClient();

    private readonly UserManager<User> _userManager;
    private readonly ApplicationContext _context;

    public UserController(UserManager<User> userManager, ApplicationContext context, UserService userService)
    {
        _userManager = userManager;
        _context = context;
        _userService = userService;
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
    //POST: User/CompleteProfile
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CompleteProfile (string id, [Bind("Id,Bio,Age,Id_Faculty,FacultyYear,Sex,Employed,Employment,Smoker,Pets,Id_Language,LeaseDuration,Id_Hobby")] User user)
    {        
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

                /* adding users to the Matches table */

                string existingUserId = existingUser.Id;

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
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine("Error sending data to the Flask API: {0}", ex.Message);
                    return View("Error");
                }

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var recommendations = JsonConvert.DeserializeObject<dynamic>(responseJson);

                foreach (var recommendation in recommendations.data)
                {
                    string recommendedUserId = recommendation[0]; // The ID of the recommended user
                    double similarityScore = recommendation[1]; // The similarity score

                    var match1 = new Match 
                    {
                        Id_User1 = existingUser.Id,
                        Id_User2 = recommendedUserId,
                        points = similarityScore, // Assuming 'Points' is used to store similarity score
                        User1 = existingUser,
                        User2 = await _context.Users.FindAsync(recommendedUserId) // Fetch the user from the context
                    };
                    _context.Matches.Add(match1);

                    var match2 = new Match 
                    {
                        Id_User2 = existingUser.Id,
                        Id_User1 = recommendedUserId,
                        points = similarityScore,
                        User1 = await _context.Users.FindAsync(recommendedUserId),
                        User2 = existingUser
                    };
                    _context.Matches.Add(match2);
                }

                await _context.SaveChangesAsync();
                
                if(User.IsInRole("Taker"))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("MakeProperty", "Property", new { userId = user.Id });
                }
            }
            else
            {
                Console.WriteLine("Flask API call failed with status code {0}", response.StatusCode);
                return View("Error");
            }
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) {}
        /*if (User.IsInRole("Giver"))
        {
            return RedirectToAction("MakeProperty", "Property", new { userId = user.Id });
        }else{
            return RedirectToAction("Index", "Home");
        }*/
    }
    return View(user);

}

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
        }    
    }