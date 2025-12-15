using Microsoft.AspNetCore.Identity;

namespace Seeding
{
    internal static class AuthenticationRoles
    {
        public static async Task Seed(RoleManager<IdentityRole> roleManager)
        {
            foreach (var roleName in Constants.AuthRoles.AsList())
            {
                if (await roleManager.RoleExistsAsync(roleName))
                {
                    continue;
                }

                await roleManager.CreateAsync(new() { Name = roleName });
            }
        }
    }
}
