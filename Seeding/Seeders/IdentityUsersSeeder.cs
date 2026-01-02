using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Identity;
using Persistence;
using System.Collections.Concurrent;

namespace Seeding.Seeders
{
    internal class IdentityUsersSeeder
    {
        private readonly ConcurrentBag<IdentityUser> _identityUsersBag = new();

        public async Task BulkInsertAsync(RepositoryDbContext context)
        {
            await context.BulkInsertAsync(
                _identityUsersBag,
                new BulkConfig
                {
                    PreserveInsertOrder = false,
                    UpdateByProperties = new List<string>
                    {
                        nameof(IdentityUser.Id)
                    }
                });
        }

        public IEnumerable<IdentityUser> Create<T>(IEnumerable<T> entities, Func<T, string> emailGetter, string passwordHash)
        {
            var identityUsers = entities.Select(e =>
            {
                var email = emailGetter(e);
                var normalizedEmail = email.ToUpper();

                return new IdentityUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = email,
                    Email = email,
                    NormalizedUserName = normalizedEmail,
                    NormalizedEmail = normalizedEmail,
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    AccessFailedCount = 0,
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    PasswordHash = passwordHash
                };
            }).ToList();

            identityUsers.ForEach(_identityUsersBag.Add);

            return identityUsers;
        }
    }
}
