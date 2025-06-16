using HospitalManagementSystem2.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem2.Data
{
    public class Seeding
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public Seeding(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task SeedRoles()
        {
            foreach (var role in Constants.AuthRoles.List)
            {
                if (await _roleManager.RoleExistsAsync(role))
                    continue;

                var result = await _roleManager.CreateAsync(new(role));
                ResultHelper.CheckIdentityResult(result);
            }
        }
    }
}
