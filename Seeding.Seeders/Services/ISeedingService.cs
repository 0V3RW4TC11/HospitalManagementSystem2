namespace Seeding.Seeders.Services
{
    public interface ISeedingService
    {
        Task SeedAdminsAsync(int amount, string password);

        Task SeedPatientsAsync(int amount, string password);

        Task SeedDoctorsAsync(int amount, string password);

        Task SeedAllAsync(int amount, string password);

        Task<bool> HasDataAsync();

        Task ResetDatabaseAsync();
    }
}
