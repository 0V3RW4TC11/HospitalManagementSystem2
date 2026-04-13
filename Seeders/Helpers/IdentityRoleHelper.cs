using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Seeders.Helpers
{
    internal class IdentityRoleHelper
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public IdentityRoleHelper(IServiceProvider services)
        {
            _roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        }

        public async Task<string> GetRoleIdAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName)
                ?? throw new InvalidOperationException($"Role '{roleName}' has not been seeded.");

            return role.Id;
        }
    }
}
