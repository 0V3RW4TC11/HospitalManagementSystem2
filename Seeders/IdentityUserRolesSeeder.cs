using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Identity;
using Persistence;
using System.Collections.Concurrent;

namespace Seeders
{
    internal class IdentityUserRolesSeeder
    {
        private readonly ConcurrentBag<IdentityUserRole<string>> _identityUserRolesBag = new();

        public async Task BulkInsertAsync(RepositoryDbContext context)
        {
            await context.BulkInsertAsync(
                _identityUserRolesBag,
                new BulkConfig
                {
                    PreserveInsertOrder = false,
                    UpdateByProperties = new List<string>
                    {
                        nameof(IdentityUserRole<string>.UserId),
                        nameof(IdentityUserRole<string>.RoleId)
                    }
                });
        }

        public void Create(IEnumerable<IdentityUser> identityUsers, string roleId)
        {
            var identityUserRoles = identityUsers.Select(u => new IdentityUserRole<string>
            {
                UserId = u.Id,
                RoleId = roleId
            }).ToList();

            identityUserRoles.ForEach(_identityUserRolesBag.Add);
        }
    }
}
