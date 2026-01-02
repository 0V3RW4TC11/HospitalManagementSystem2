using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Seeding.Helpers
{
    internal static class IdentityRoleHelper
    {
        public static async Task<IdentityRole> GetRoleAsync(IServiceProvider services, string roleName)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var role = await roleManager.FindByNameAsync(roleName);

            if (role is not null)
            {
                return role;
            }

            role = new IdentityRole(roleName);

            var result = await roleManager.CreateAsync(role);
            if (result.Succeeded is false)
                throw new Exception($"Seeding role {roleName} failed. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            return role;
        }
    }
}
