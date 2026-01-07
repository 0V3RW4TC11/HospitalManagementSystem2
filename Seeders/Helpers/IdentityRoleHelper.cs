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
            return await _roleManager.GetRoleIdAsync(new IdentityRole(roleName));
        }
    }
}
