using Bogus;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Seeding.Seeders
{
    internal static class AdminsSeeder
    {
        public static async Task SeedAsync(
            IDbContextFactory<RepositoryDbContext> factory,
            int totalUsers,
            int batchSize,
            int parallelism)
        {
            using var context = await factory.CreateDbContextAsync();
            var exists = await context.Admins.AnyAsync();
            if (exists)
            {
                Console.WriteLine("Admins already seeded");
                return;
            }

            int batches = (totalUsers + batchSize - 1) / batchSize;
            var faker = GetAdminFaker();

            await Parallel.ForEachAsync(
                Enumerable.Range(0, batches),
                new ParallelOptions { MaxDegreeOfParallelism = parallelism },
                async (batchIndex, ct) =>
                {
                    var adminsInBatch = Math.Min(batchSize, totalUsers - batchIndex * batchSize);
                    var admins = faker.Generate(adminsInBatch);
                    await using var db = await factory.CreateDbContextAsync(ct);

                    db.Admins.AddRange(admins);
                    await db.SaveChangesAsync(ct);
                    Console.WriteLine($"Seeded batch {batchIndex + 1}/{batches}. {adminsInBatch} in batch.");
                });

            Console.WriteLine("Admin seeding complete.");
        }

        private static Faker<Admin> GetAdminFaker()
        {
            return new Faker<Admin>()
                .CustomInstantiator(f =>
                {
                    var genderEnum = f.PickRandom<Bogus.DataSets.Name.Gender>();
                    var firstName = f.Name.FirstName(genderEnum);
                    var lastName = f.Name.LastName(genderEnum);
                    var gender = genderEnum == Bogus.DataSets.Name.Gender.Male ? "Male" : "Female";

                    return new Admin
                    {
                        Title = f.Name.Prefix(genderEnum),
                        FirstName = firstName,
                        LastName = lastName,
                        Gender = gender,
                        Address = f.Address.FullAddress(),
                        Phone = f.Phone.PhoneNumber(),
                        Email = f.Internet.Email(firstName, lastName),
                        DateOfBirth = DateOnly.FromDateTime(f.Person.DateOfBirth)
                    };
                });
        }
    }
}
