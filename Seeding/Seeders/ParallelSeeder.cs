using Bogus;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seeding.Seeders
{
    internal static class ParallelSeeder
    {
        public static async Task SeedAsync<T>(
            IDbContextFactory<RepositoryDbContext> factory,
            Faker<T> faker,
            int totalEntities,
            int batchSize,
            int parallelism
            ) where T : class
        {
            using var context = await factory.CreateDbContextAsync();
            bool exists = await context.Set<T>().AnyAsync();
            if (exists)
            {
                Console.WriteLine($"{nameof(T)}s already seeded");
                return;
            }

            int batches = (totalEntities + batchSize - 1) / batchSize;
            
            await Parallel.ForEachAsync(
                Enumerable.Range(0, batches)),
                new ParallelOptions { MaxDegreeOfParallelism = parallelism },
                async (batchIndex, ct) =>
                {
                    var entitiesInBatch = Math.Min(batchSize, totalUsers - batchIndex * batchSize);
                    var entities = faker.Generate(entitiesInBatch);

                }
        }
    }
}
