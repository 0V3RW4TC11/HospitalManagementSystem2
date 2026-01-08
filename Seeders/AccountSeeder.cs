using Domain.Entities;
using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System.Collections.Concurrent;

namespace Seeders
{
    internal class AccountSeeder : IDisposable
    {
        private readonly ConcurrentBag<Account> _accountsBag = new();

        public void Create<T>(IEnumerable<T> entities, IEnumerable<IdentityUser> identityUsers) where T : Entity
        {
            var accounts = entities.Zip(identityUsers, (entity, user) => new Account
            {
                UserId = entity.Id,
                IdentityUserId = user.Id
            }).ToList();

            accounts.ForEach(_accountsBag.Add);
        }

        public async Task BulkInsertAsync(RepositoryDbContext context)
        {
            await context.BulkInsertAsync(_accountsBag, new BulkConfig { PreserveInsertOrder = false });
        }

        public void Dispose()
        {
            _accountsBag.Clear();
        }
    }
}
