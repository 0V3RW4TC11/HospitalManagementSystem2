using ConsoleTools;
using Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Services.Abstractions;

namespace Seeding
{
    internal class Menu
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IServiceManager _serviceManager;
        private readonly IRepositoryManager _repositoryManager;
        private readonly ConsoleMenu _consoleMenu;

        public Menu(ServiceProvider provider)
        {
            _roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            _serviceManager = provider.GetRequiredService<IServiceManager>();
            _repositoryManager = provider.GetRequiredService<IRepositoryManager>();

            _consoleMenu = new ConsoleMenu([], level: 0);
            _consoleMenu
                .AddRange(GetMenuItems())
                .Add("Exit", ConsoleMenu.Close)
                .Configure(ConfigureMenu(_consoleMenu));
        }

        public void Run()
        {
            _consoleMenu.Show();
        }

        private Tuple<string, Func<CancellationToken, Task>>[] GetMenuItems()
        {
            return
            [
                Tuple.Create("Seed Authentication Roles", TryAction(async () =>
                {
                    await Actions.SeedAuthRoles(_roleManager);
                    Console.WriteLine("Roles seeded");
                    PressAny();
                })),
                Tuple.Create("Seed Specializations", TryAction(async () =>
                {
                    await Actions.SeedSpecializations(_serviceManager.SpecializationService);
                    Console.WriteLine("Specializations seeded");
                    PressAny();
                })),
                Tuple.Create("Seed Doctors", TryAction(async () =>
                {
                    await Actions.SeedDoctors(
                        _serviceManager.DoctorService,
                        _serviceManager.SpecializationService);
                    Console.WriteLine("Doctors seeded");
                    PressAny();
                })),
                Tuple.Create("Seed Patients", TryAction(async () =>
                {
                    await Actions.SeedPatients(_serviceManager.PatientService);
                    Console.WriteLine("Patients seeded");
                    PressAny();
                })),
                Tuple.Create("Create Admin", TryAction(async () =>
                {
                    await Actions.CreateAdmin(_serviceManager.AdminService);
                    Console.WriteLine("Admin created");
                    PressAny();
                }))
            ];
        }

        private static Action<MenuConfig> ConfigureMenu(ConsoleMenu menu) => 
            (config) =>
            {
                config.EnableWriteTitle = true;
                config.Title = $"Database Seeding Menu\n";
                config.WriteHeaderAction = () => Console.WriteLine("Select option (use arrow keys):");
                config.WriteItemAction = item => Console.Write($" {item.Name}");
                config.SelectedItemBackgroundColor = Console.BackgroundColor;
                config.SelectedItemForegroundColor = Console.ForegroundColor;
            };

        private static void PressAny()
        {
            Console.WriteLine("Press any key to continue ...");
            Console.ReadKey();
        }

        private static Func<CancellationToken, Task> TryAction(Func<Task> action)
        {
            return async (token) =>
            {
                try
                {
                    await action();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    PressAny();
                }
            };
        }
    }
}
