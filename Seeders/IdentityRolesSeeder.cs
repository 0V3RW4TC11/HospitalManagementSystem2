using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Persistence;

namespace Seeders
{
    internal static class IdentityRolesSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<RepositoryDbContext>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            var rolesAll = Constants.AuthRoles.AsArray();
            var rolesExisting = context.Roles.Select(r => r.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var rolesToInsert = rolesAll.Except(rolesExisting, StringComparer.OrdinalIgnoreCase);

            foreach (var role in rolesToInsert)
            {
                await roleManager.CreateAsync(new IdentityRole(role!));
            }
        }
    }
}
