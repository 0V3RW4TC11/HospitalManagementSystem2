using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Services.Abstractions;

namespace Seeding.Seeders
{
    internal class DataSeeder : IDataSeeder
    {
        private readonly ServiceProvider _provider;
        private readonly string _pathToDoctorSpecsCsv;

        public DataSeeder(
            ServiceProvider provider, 
            string pathToDoctorSpecsCsv)
        {
            _provider = provider;
            _pathToDoctorSpecsCsv = pathToDoctorSpecsCsv;
        }

        public async Task SeedAdminsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task SeedAuthenticationRolesAsync()
        {
            var roleManager = _provider.GetRequiredService<RoleManager<IdentityRole>>();
            await AuthenticationRolesSeeder.Seed(roleManager);
        }

        public async Task SeedDoctorsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task SeedDoctorSpecializationsAsync()
        {
            var service = _provider.GetRequiredService<IServiceManager>().SpecializationService;
            await DoctorSpecializationsSeeder.Seed(service, _pathToDoctorSpecsCsv);
        }

        public async Task SeedPatientsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
