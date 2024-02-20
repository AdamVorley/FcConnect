using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace FcConnect.Data
{
    public class SeedData
    {
        public static async Task Initialize(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "User", "Manager" };

            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                // Check if the role exists
                var roleExist = await roleManager.RoleExistsAsync(roleName);

                if (!roleExist)
                {
                    // Create the roles and seed them to the database
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
}
