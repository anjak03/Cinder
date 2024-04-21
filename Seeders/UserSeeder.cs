using Microsoft.AspNetCore.Identity;
using Cinder.Models;
using System.Threading.Tasks;

namespace Cinder.Seeders
{
    /// <summary>
    /// Seeds users into the database if they do not already exist.
    /// </summary>
    public class UserSeeder
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            var roles = new[] { "Admin", "Giver", "Taker" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        public static async Task SeedUsersAsync(UserManager<User> userManager)
        {
            // Admin User
            await CreateUserIfNotExists(userManager, "admin@cinder.com", "admin", "Admin123*", "Admin");

            // Other Users
            for (int i = 0; i < 100; i++)
            {
                string email = $"giver{i}@cinder.com";
                string username = $"giver{i}";
                await CreateUserIfNotExists(userManager, email, username, "User123*", "Giver");
            }

            for (int i = 0; i < 100; i++)
            {
                string email = $"taker{i}@cinder.com";
                string username = $"taker{i}";
                await CreateUserIfNotExists(userManager, email, username, "User123*", "Taker");
            }
        }

        private static async Task CreateUserIfNotExists(UserManager<User> userManager, string email, string username, string password, string role)
        {
            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new User { Email = email, UserName = username, EmailConfirmed = true };
                await userManager.CreateAsync(user, password);
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}
