using Bogus;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seeding.Seeders
{
    internal static class AdminsSeeder
    {
        private static readonly string[] _gender = ["Male", "Female"];

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


        }

        private static Faker<Admin> GetAdminFaker()
        {
            var faker = new Faker<Admin>()
                .RuleFor(a => a.Gender, f => f.PickRandom(_gender))
                .RuleFor(a => a.FirstName, f => f.Name.FirstName())
                .RuleFor(a => a.FirstName, f => f.Name.FirstName())
        }
    }
}
