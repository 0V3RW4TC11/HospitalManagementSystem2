using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Seeding.Helpers
{
    internal class IdentityRoleHelper
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public IdentityRoleHelper(IServiceProvider services)
        {
            _roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        }

        public async Task<IdentityRole> GetRoleAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);

            if (role is not null)
            {
                return role;
            }

            role = new IdentityRole(roleName);

            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded is false)
                throw new Exception($"Seeding role {roleName} failed. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            return role;
        }
    }
}
