namespace Seeding.Seeders
{
    internal interface ISeeder : IDisposable
    {
        Task SeedAsync(int amount);
    }
}