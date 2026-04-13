namespace Seeders
{
    internal interface ISeeder : IDisposable
    {
        Task SeedAsync(int amount);
    }
}