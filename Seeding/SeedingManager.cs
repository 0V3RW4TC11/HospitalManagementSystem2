using Microsoft.Extensions.DependencyInjection;
using Persistence;
using Seeding.Helpers;
using Seeding.Seeders;

namespace Seeding
{
    internal class SeedingManager
    {
        private readonly IServiceProvider _services;
        private readonly IdentityRoleHelper _roleHelper;

        public SeedingManager(IServiceProvider services)
        {
            _services = services;
            _roleHelper = new IdentityRoleHelper(_services);
        }

        public async Task SeedAdminsAsync(int amount, string password)
        {
            var adminSeeder = await CreateAdminSeeder(password);
            await adminSeeder.SeedAsync(amount);
        }

        public async Task SeedPatientsAsync(int amount, string password)
        {
            var patientSeeder = await CreatePatientSeeder(password);
            await patientSeeder.SeedAsync(amount);
        }

        public async Task SeedDoctorsAsync(int amount, string password)
        {
            var doctorSeeder = await CreateDoctorSeeder(password);
            await doctorSeeder.SeedAsync(amount);
        }

        public async Task SeedAllAsync(int amount, string password)
        {
            var seeders = new List<ISeeder>
            {
                await CreateAdminSeeder(password),
                await CreatePatientSeeder(password),
                await CreateDoctorSeeder(password)
            };

            seeders.ForEach(async s => await s.SeedAsync(amount));
        }

        private async Task<AdminSeeder> CreateAdminSeeder(string password)
        {
            var role = await _roleHelper.GetRoleAsync(Constants.AuthRoles.Admin);
            return new AdminSeeder(_services, role.Id, password);
        }

        private async Task<PatientSeeder> CreatePatientSeeder(string password)
        {
            var role = await _roleHelper.GetRoleAsync(Constants.AuthRoles.Patient);
            return new PatientSeeder(_services, role.Id, password);
        }

        private async Task<DoctorSeeder> CreateDoctorSeeder(string password)
        {
            var role = await _roleHelper.GetRoleAsync(Constants.AuthRoles.Doctor);
            var context = _services.GetRequiredService<RepositoryDbContext>();
            var specIds = await SpecializationsSeeder.SeedAsync(
                context,
                Path.Combine(AppContext.BaseDirectory, "Data", "specializations.csv"));

            return new DoctorSeeder(_services, specIds, role.Id, password);
        }
    }
}
