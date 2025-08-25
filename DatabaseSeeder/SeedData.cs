using DataTransfer.Admin;
using Domain.Constants;
using Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Services.Abstractions;

namespace Seeding
{
    public static class SeedData
    {
        public static async Task SeedAuthRoles(RoleManager<IdentityRole> manager)
        {
            foreach (var roleName in AuthRoles.AsList())
            {
                bool roleExists = await manager.RoleExistsAsync(roleName);
                
                if (roleExists is false)
                {
                    await manager.CreateAsync(new() { Name = roleName });
                }
            }
        }

        public static async Task SeedAdmin(IAdminService service)
        {
            var admin = new AdminCreateDto
            {
                Title = "Mr",
                FirstName = "Test",
                LastName = "Admin",
                Gender = "Male",
                Address = "1 Example Street, ExampleCity EX 0000",
                Phone = "00-0-0000-0000",
                Email = "test.admin@example.com",
                DateOfBirth = DateOnly.FromDateTime(DateTime.UnixEpoch),
                Password = "Password123!"
            };

            try
            {
                await service.CreateAsync(admin);
            }
            catch (AdminDuplicationException)
            {
            }
        }

        public static async Task SeedSpecializations(ISpecializationService service)
        {
            throw new NotImplementedException();
        }

        public static async Task SeedDoctors(IDoctorService service)
        {
            throw new NotImplementedException();
        }

        public static async Task SeedPatients(IPatientService service)
        {
            throw new NotImplementedException();
        }

        public static async Task SeedAttendances(IAttendanceService service)
        {
            throw new NotImplementedException();
        }
    }
}
