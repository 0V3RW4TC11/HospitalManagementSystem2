using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using Seeders.Helpers;

namespace Seeders.Services
{
    public class SeedingService : ISeedingService
    {
        private DatabaseService _databaseService;
        private IdentityRoleHelper _roleHelper;

        private SeedingService()
        { }

        public static async Task<SeedingService> CreateAsync()
        {
            var service = new SeedingService();
            await service.InitializeAsync();
            return service;
        }

        public async Task<bool> HasDataAsync()
        {
            var context = _databaseService.Services.GetRequiredService<HmsDbContext>();

            if (await context.Admins.AnyAsync())
                return true;
            if (await context.Patients.AnyAsync())
                return true;
            if (await context.Doctors.AnyAsync())
                return true;
            if (await context.DoctorSpecializations.AnyAsync())
                return true;
            if (await context.Specializations.AnyAsync())
                return true;
            if (await context.Attendances.AnyAsync())
                return true;
            if (await context.Users.AnyAsync())
                return true;
            if (await context.UserRoles.AnyAsync())
                return true;
            if (await context.UserClaims.AnyAsync())
                return true;

            return false;
        }

        public async Task ResetDatabaseAsync()
        {
            if (_databaseService is null)
                throw new InvalidOperationException("Service is not initialized.");

            var context = _databaseService.Services.GetRequiredService<HmsDbContext>();

            await Task.WhenAll(
                context.Admins.ExecuteDeleteAsync(),
                context.Patients.ExecuteDeleteAsync(),
                context.Doctors.ExecuteDeleteAsync(),
                context.DoctorSpecializations.ExecuteDeleteAsync(),
                context.Specializations.ExecuteDeleteAsync(),
                context.Attendances.ExecuteDeleteAsync(),
                context.Users.ExecuteDeleteAsync(),
                context.UserClaims.ExecuteDeleteAsync(),
                context.UserRoles.ExecuteDeleteAsync()
            );
        }

        public async Task SeedAdminsAsync(int amount, string password)
        {
            using var adminSeeder = await CreateAdminSeederAsync(password);
            await adminSeeder.SeedAsync(amount);
        }

        public async Task SeedAllAsync(int amount, string password)
        {
            // Execute seeders sequentially to avoid memory pressure
            using (var adminSeeder = await CreateAdminSeederAsync(password))
            {
                await adminSeeder.SeedAsync(amount);
            }
            
            using (var patientSeeder = await CreatePatientSeederAsync(password))
            {
                await patientSeeder.SeedAsync(amount);
            }
            
            using (var doctorSeeder = await CreateDoctorSeederAsync(password))
            {
                await doctorSeeder.SeedAsync(amount);
            }

            // Force garbage collection to clean up Bogus internal state
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        public async Task SeedDoctorsAsync(int amount, string password)
        {
            using var doctorSeeder = await CreateDoctorSeederAsync(password);
            await doctorSeeder.SeedAsync(amount);
        }

        public async Task SeedPatientsAsync(int amount, string password)
        {
            using var patientSeeder = await CreatePatientSeederAsync(password);
            await patientSeeder.SeedAsync(amount);
        }

        private async Task<AdminSeeder> CreateAdminSeederAsync(string password)
        {
            var roleId = await _roleHelper.GetRoleIdAsync(Constants.AuthRoles.Admin);
            return new AdminSeeder(_databaseService.Services, roleId, password);
        }

        private async Task<DoctorSeeder> CreateDoctorSeederAsync(string password)
        {
            var roleId = await _roleHelper.GetRoleIdAsync(Constants.AuthRoles.Doctor);
            var services = _databaseService.Services;
            var context = services.GetRequiredService<HmsDbContext>();
            var specIds = await SpecializationsSeeder.SeedAsync(
                context,
                Path.Combine(AppContext.BaseDirectory, "Data", "specializations.csv"));

            return new DoctorSeeder(services, specIds, roleId, password);
        }

        private async Task<PatientSeeder> CreatePatientSeederAsync(string password)
        {
            var roleId = await _roleHelper.GetRoleIdAsync(Constants.AuthRoles.Patient);
            return new PatientSeeder(_databaseService.Services, roleId, password);
        }

        private async Task InitializeAsync()
        {
            _databaseService = new DatabaseService();

            Console.Write("Initializing database...");
            await _databaseService.InitializeDatabaseAsync();
            Console.WriteLine("OK!");

            Console.Write("Seeding Roles...");
            await IdentityRolesSeeder.SeedAsync(_databaseService.Services);
            Console.WriteLine("OK!");

            _roleHelper = new IdentityRoleHelper(_databaseService.Services);

            Console.WriteLine("\nSeeding service initialization complete.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
