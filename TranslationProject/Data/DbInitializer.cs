using Microsoft.EntityFrameworkCore;
using TranslationModelLibrary.Context;
using Microsoft.AspNetCore.Identity;
using TranslationProject.Models;
using System.Security.Claims;

namespace TranslationProject.Data
{
    public class DbInitializer
    {
        static async Task AssignRoleToUser(UserManager<TranslationUser> userManager, string email, string role)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return;
            }

            // Check if the user already has the role
            if (await userManager.IsInRoleAsync(user, role))
            {
                return;
            }

            IdentityResult roleResult = await userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
            {
                throw new Exception("Failed to add role to user");
            }
        }

        public static async Task Initialize(UserManager<TranslationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Ensure roles exist
            string[] roles = new string[] { "Admin", "Premium", "Basic" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Create dummy users
            var users = new List<(string Email, string Password, string Role, string PreferredLanguage)>
            {
                ("admin@test.com", "Admin123!", "Admin", "en"),
                ("test@test.com", "Test123!", "Basic", "en"),
                ("test_premium@test.com", "Premium123!", "Premium", "es"),
                ("user1@test.com", "User123!", "Basic", "fr"),
                ("user2@test.com", "User123!", "Premium", "de")
            };

            foreach (var (email, password, role, preferredLanguage) in users)
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new TranslationUser { UserName = email, Email = email, PreferredLanguage = preferredLanguage };
                    var result = await userManager.CreateAsync(user, password);
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Failed to create user {email}");
                    }
                }

                await AssignRoleToUser(userManager, email, role);
            }
        }
    }
}