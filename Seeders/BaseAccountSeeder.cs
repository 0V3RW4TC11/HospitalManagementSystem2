using Bogus;
using Domain.Entities;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using Seeders.Helpers;
using System.Collections.Concurrent;

namespace Seeders
{
    internal abstract class BaseAccountSeeder<T> : ISeeder where T : Entity
    {
        protected readonly IDbContextFactory<RepositoryDbContext> _contextFactory;
        private readonly AccountSeeder _accountSeeder = new();
        private readonly Func<T, string> _emailAccessor;
        private readonly ConcurrentBag<T> _entitiesBag = new();
        private readonly Faker<T> _faker;
        private readonly IdentityUserRolesSeeder _identityRolesSeeder = new();
        private readonly IdentityUsersSeeder _identityUsersSeeder = new();
        private readonly string _passwordHash;
        private readonly string _roleId;

        public BaseAccountSeeder(
            IServiceProvider services,
            string roleId,
            string password,
            Func<T, string> emailAccessor)
        {
            _contextFactory = services.GetRequiredService<IDbContextFactory<RepositoryDbContext>>();
            _emailAccessor = emailAccessor;
            _faker = CreateFaker();
            _passwordHash = IdentityPasswordHashHelper.HashPassword(services, password);
            _roleId = roleId;
        }

        public async Task SeedAsync(int amount)
        {
            int parallelism = Math.Max(1, Environment.ProcessorCount / 2);
            int batchSize = Math.Min(1000, Math.Max(1, amount / parallelism));
            int numBatches = (amount + batchSize - 1) / batchSize;

            Parallel.ForEach(
                Enumerable.Range(0, numBatches),
                new ParallelOptions { MaxDegreeOfParallelism = parallelism },
                batchIndex =>
                {
                    int batchAmount = Math.Min(batchSize, amount - batchIndex * batchSize);

                    var entities = BatchFunc(batchAmount);
                    entities.ForEach(_entitiesBag.Add);

                    var identityUsers = _identityUsersSeeder.Create(entities, _emailAccessor, _passwordHash);
                    _identityRolesSeeder.Create(identityUsers, _roleId);
                    _accountSeeder.Create(entities, identityUsers);
                });

            await Task.WhenAll(CreateInsertTasks());
        }

        protected virtual List<T> BatchFunc(int batchAmount)
        {
            return _faker.Generate(batchAmount);
        }

        protected abstract Faker<T> CreateFaker();

        protected virtual List<Task> CreateInsertTasks()
        {
            return new List<Task>
            {
                AsyncHelper.InvokeAsync(async () =>
                {
                    await using var context = await _contextFactory.CreateDbContextAsync();
                    await context.BulkInsertAsync(_entitiesBag, new BulkConfig { PreserveInsertOrder = false });
                }),
                AsyncHelper.InvokeAsync(async () =>
                {
                    await using var context = await _contextFactory.CreateDbContextAsync();
                    await _identityUsersSeeder.BulkInsertAsync(context);
                }),
                AsyncHelper.InvokeAsync(async () =>
                {
                    await using var context = await _contextFactory.CreateDbContextAsync();
                    await _identityRolesSeeder.BulkInsertAsync(context);
                }),
                AsyncHelper.InvokeAsync(async () =>
                {
                    await using var context = await _contextFactory.CreateDbContextAsync();
                    await _accountSeeder.BulkInsertAsync(context);
                })
            };
        }
    }
}
