namespace Seeders
{
    internal interface ISeeder
    {
        Task SeedAsync(int amount);
    }
}