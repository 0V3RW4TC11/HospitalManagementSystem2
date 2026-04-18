using EFCore.BulkExtensions;
using Entities;
using Microsoft.AspNetCore.Identity;
using Persistence;
using Persistence.AppConstants;
using System.Collections.Concurrent;

namespace Seeding.Seeders
{
    internal class IdentityClaimsSeeder : IDisposable
    {
        private readonly ConcurrentBag<IdentityUserClaim<string>> _claimsBag = new();

        public void Create(IEnumerable<Entity> entities, IEnumerable<IdentityUser> identityUsers)
        {
            var accounts = entities.Zip(identityUsers, (entity, user) => new IdentityUserClaim<string>
            {
                UserId = user.Id,
                ClaimType = ClaimConstants.HmsUserId,
                ClaimValue = entity.Id.ToString()
            }).ToList();

            accounts.ForEach(_claimsBag.Add);
        }

        public async Task BulkInsertAsync(HmsDbContext context)
        {
            await context.BulkInsertAsync(_claimsBag, new BulkConfig { PreserveInsertOrder = false });
        }

        public void Dispose()
        {
            _claimsBag.Clear();
        }
    }
}
