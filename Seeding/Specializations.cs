using Domain.Exceptions;
using Services.Abstractions;

namespace Seeding
{
    internal static class Specializations
    {
        public static async Task Seed(ISpecializationService service, string pathToCsv)
        {
            foreach (var line in File.ReadLines(pathToCsv))
            {
                try
                {
                    await service.CreateAsync(new() { Name = line.Trim() });
                }
                catch (SpecializationDuplicationException)
                {
                }
            }
        }
    }
}
