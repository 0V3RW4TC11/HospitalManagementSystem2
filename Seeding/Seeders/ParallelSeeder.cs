using Bogus;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Seeding.Seeders
{
    internal static class ParallelSeeder
    {
        public static async Task SeedAsync<T>(
            Func<int, CancellationToken, Task> insertActionAsync,
            int totalEntities,
            int batchSize,
            int parallelism) where T : class
        {
            int batches = (totalEntities + batchSize - 1) / batchSize;
            
            await Parallel.ForEachAsync(
                Enumerable.Range(0, batches),
                new ParallelOptions { MaxDegreeOfParallelism = parallelism },
                async (batchIndex, ct) =>
                {
                    var entitiesInBatch = Math.Min(batchSize, totalEntities - batchIndex * batchSize);
                    await insertActionAsync(entitiesInBatch, ct);
                    Console.WriteLine($"Seeded batch {batchIndex + 1}/{batches}. {entitiesInBatch} in batch.");
                });
        }
    }
}
