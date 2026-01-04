using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Persistence;

namespace Seeding.Seeders
{
    internal class IdentityRolesManager
    {
        public Dictionary<string, string> RoleIds { get; } = new();

        public async Task SeedAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<RepositoryDbContext>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            var rolesAll = Constants.AuthRoles.AsArray();
            var rolesExisting = context.Roles.Select(r => r.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var rolesToInsert = rolesAll.Except(rolesExisting, StringComparer.OrdinalIgnoreCase);

            foreach (var role in rolesToInsert)
            {
                var identityRole = new IdentityRole(role!);
                await roleManager.CreateAsync(identityRole);
                RoleIds[role!] = identityRole.Id;
            }
        }
    }
}
