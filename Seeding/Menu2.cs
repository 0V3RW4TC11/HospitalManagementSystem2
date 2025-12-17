using ConsoleTools;

namespace Seeding
{
    internal class Menu2
    {
        private readonly ConsoleMenu _menu;
        
        public Menu2(IDataSeeder seeder)
        {
            _menu = new ConsoleMenu([], level: 0);
            _menu
                .AddRange(GetMenuItems(seeder))
                .Add("Exit", ConsoleMenu.Close)
                .Configure(ConfigureMenu(_menu));
            
        }

        public void Show() => _menu.Show();

        private static Tuple<string, Func<CancellationToken, Task>>[] GetMenuItems(IDataSeeder seeder)
        {
            return [
                Tuple.Create("Seed Authentication Roles", TryAsync(async () => {
                    await seeder.SeedAuthenticationRolesAsync();
                    PressAny();
                })),
                Tuple.Create("Seed Doctor Specializations", TryAsync(async () => {
                    await seeder.SeedDoctorSpecializationsAsync();
                    PressAny();
                })),
                Tuple.Create("Seed Admins", TryAsync(async () => {
                    await seeder.SeedAdminsAsync();
                    PressAny();
                })),
                Tuple.Create("Seed Doctors", TryAsync(async () => {
                    await seeder.SeedDoctorsAsync();
                    PressAny();
                })),
                Tuple.Create("Seed Patients", TryAsync(async () => {
                    await seeder.SeedPatientsAsync();
                    PressAny();
                })),
            ];
        }

        private static void PressAny()
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static Func<CancellationToken, Task> TryAsync(Func<Task> action)
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

        private static Action<MenuConfig> ConfigureMenu(ConsoleMenu menu)
        {
            return (config) =>
            {
                config.EnableWriteTitle = true;
                config.Title = $"Database Seeding Menu\n";
                config.WriteHeaderAction = () => Console.WriteLine("Select option (use arrow keys):");
                config.WriteItemAction = item => Console.Write($" {item.Name}");
                config.SelectedItemBackgroundColor = Console.BackgroundColor;
                config.SelectedItemForegroundColor = Console.ForegroundColor;
            };
        }
    }
}
