namespace Seeding.Seeders
{
    internal interface ISeeder
    {
        Task SeedAsync(int amount);
    }
}