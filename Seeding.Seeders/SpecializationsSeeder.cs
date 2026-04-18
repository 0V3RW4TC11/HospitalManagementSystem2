using Persistence;

namespace Seeding.Seeders
{
    internal static class SpecializationsSeeder
    {
        public static async Task<IEnumerable<Guid>> SeedAsync(HmsDbContext context, string pathToCsv)
        {
            var specializationNames = File.ReadLines(pathToCsv)
                .Select(line => line.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .ToList();

            var specializations = specializationNames.Select(name => new Entities.Specialization
            {
                Id = Guid.NewGuid(),
                Name = name
            }).ToList();

            await context.Specializations.AddRangeAsync(specializations);
            await context.SaveChangesAsync();

            return specializations.Select(s => s.Id);
        }
    }
}
