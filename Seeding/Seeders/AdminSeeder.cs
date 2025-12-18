using Bogus;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Seeding.Helpers;
using static Bogus.DataSets.Name;

namespace Seeding.Seeders
{
    internal class AdminSeeder
    {
        private readonly Faker<Admin> _faker;

        public AdminSeeder()
        {
            _faker = new Faker<Admin>()
                .RuleFor(a => a.Gender, f => f.PickRandom<Gender>() == Gender.Male ? "Male" : "Female")
                .RuleFor(a => a.Title, (f, a) => f.Name.Prefix(GetGender(a.Gender!)))
                .RuleFor(a => a.FirstName, (f, a) => f.Name.FirstName(GetGender(a.Gender!)))
                .RuleFor(a => a.LastName, (f, a) => f.Name.LastName(GetGender(a.Gender!)))
                .RuleFor(a => a.Address, f => f.Address.FullAddress())
                .RuleFor(a => a.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(a => a.Email, (f, a) => f.Internet.Email(a.FirstName, a.LastName))
                .RuleFor(a => a.DateOfBirth, f => DateOnly.FromDateTime(f.Person.DateOfBirth));
        }

        public async Task SeedAsync(
            IDbContextFactory<RepositoryDbContext> factory,
            int totalAdmins,
            int batchSize,
            int parallelism)
        {
            if (await DatabaseHelper.HasDataAsync<Admin>(factory))
            {
                Console.WriteLine("Admins already populated.");
                return;
            }

            await ParallelSeeder.SeedAsync<Admin>(
                async (adminsToBatch, ct) =>
                {
                    // EXAMPLE!
                    // TODO: Use batching into Admins table and Identity framework core bulk extensions for better performance
                    var admins = _faker.Generate(adminsToBatch);
                    await using var db = await factory.CreateDbContextAsync(ct);
                    db.Set<Admin>().AddRange(admins);
                    await db.SaveChangesAsync(ct);
                },
                totalAdmins,
                batchSize,
                parallelism);

            Console.WriteLine("Admin seeding completed.");
        }

        private static Gender GetGender(string gender) =>
            gender == "Male" ? Gender.Male : Gender.Female;
    }
}
