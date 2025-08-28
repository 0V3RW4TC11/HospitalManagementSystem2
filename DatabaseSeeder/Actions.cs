using DataTransfer.Admin;
using Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Services.Abstractions;

namespace Seeding
{
    internal static class Actions
    {
        public static async Task SeedAuthRoles(RoleManager<IdentityRole> manager)
        {
            foreach (var roleName in Constants.AuthRoles.AsList())
            {
                bool roleExists = await manager.RoleExistsAsync(roleName);
                
                if (roleExists is false)
                {
                    await manager.CreateAsync(new() { Name = roleName });
                }
            }
        }

        public static async Task CreateAdmin(IAdminService service)
        {
            Console.WriteLine("Enter First Name:");
            var firstName = Console.ReadLine();
            Console.WriteLine("Enter Last Name:");
            var lastName = Console.ReadLine();
            Console.WriteLine("Enter Phone Number:");
            var phone = Console.ReadLine();
            Console.WriteLine("Enter Email:");
            var email = Console.ReadLine();
            Console.WriteLine("Enter Password:");
            var password = Console.ReadLine();

            var dto = new AdminCreateDto
            {
                FirstName = firstName,
                LastName = lastName,
                Phone = phone,
                Email = email,
                Password = password
            };

            await service.CreateAsync(dto);
        }

        public static async Task SeedSpecializations(ISpecializationService service)
        {
            foreach (var spec in Data.Specializations)
            {
                try
                {
                    await service.CreateAsync(new() { Name = spec });
                }
                catch(SpecializationDuplicationException)
                {
                }
            }
        }

        public static async Task SeedDoctors(IDoctorService docService, ISpecializationService specService)
        {
            // Get list of Specializations
            var specs = (await specService.GetAllAsync()).ToArray();
            if (specs.Length == 0)
            {
                throw new Exception("No Specializations. Seed Specializations first.");
            }

            // Initialize random num generator
            var random = new Random();

            // Seed Doctors into database
            foreach (var doctor in Data.Doctors())
            {
                // Determine random amount of Specializations to give Doctor
                var count = random.Next(1, specs.Length + 1);

                // Select Specializations at random
                for (int i = 0; i < count; i++)
                {
                    var index = random.Next(0, specs.Length);

                    doctor.SpecializationIds.Add(specs[index].Id);
                }

                try
                {
                    // Enter Doctor data into database
                    await docService.CreateAsync(doctor);
                }
                catch (DoctorDuplicationException)
                {
                }
            }
        }

        public static async Task SeedPatients(IPatientService service)
        {
            foreach (var patient in Data.Patients())
            {
                try
                {
                    await service.CreateAsync(patient);
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
