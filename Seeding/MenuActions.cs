using Microsoft.Extensions.DependencyInjection;
using Persistence;
using Seeding.Helpers;
using Spectre.Console;

namespace Seeding
{
    internal class MenuActions
    {
        private readonly RepositoryDbContext _context;
        private readonly SeedingManager _seedingManager;

        public MenuActions(IServiceProvider services)
        {
            _context = services.GetRequiredService<RepositoryDbContext>();
            _seedingManager = new SeedingManager(services);
        }

        public async Task ResetDatabaseAsync()
        {
            AnsiConsole.MarkupLine("[yellow]WARNING: This will delete all data from the database![/]");
            
            var confirm = AnsiConsole.Confirm("Are you sure you want to continue?", false);

            if (confirm)
            {
                await AnsiConsole.Status()
                    .StartAsync("Clearing database...", async ctx =>
                    {
                        await DatabaseHelper.ResetDatabase(_context);
                    });

                AnsiConsole.MarkupLine("[green]Database cleared successfully![/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[grey]Operation cancelled.[/]");
            }

            PressAnyKey();
        }

        public async Task SeedAdminsAsync()
        {
            var (amount, password) = PromptSeedingInput();
            
            await AnsiConsole.Status()
                .StartAsync($"Seeding {amount} admin(s)...", async ctx =>
                {
                    await _seedingManager.SeedAdminsAsync(amount, password);
                });

            AnsiConsole.MarkupLine($"[green]Successfully seeded {amount} admin(s)[/]");
            PressAnyKey();
        }

        public async Task SeedPatientsAsync()
        {
            var (amount, password) = PromptSeedingInput();
            
            await AnsiConsole.Status()
                .StartAsync($"Seeding {amount} patient(s)...", async ctx =>
                {
                    await _seedingManager.SeedPatientsAsync(amount, password);
                });

            AnsiConsole.MarkupLine($"[green]Successfully seeded {amount} patient(s)[/]");
            PressAnyKey();
        }

        public async Task SeedDoctorsAsync()
        {
            var (amount, password) = PromptSeedingInput();
            
            await AnsiConsole.Status()
                .StartAsync($"Seeding {amount} doctor(s)...", async ctx =>
                {
                    await _seedingManager.SeedDoctorsAsync(amount, password);
                });

            AnsiConsole.MarkupLine($"[green]Successfully seeded {amount} doctor(s)[/]");
            PressAnyKey();
        }

        public async Task SeedAllAsync()
        {
            var (amount, password) = PromptSeedingInput();
            
            await AnsiConsole.Status()
                .StartAsync($"Seeding {amount} of each user type...", async ctx =>
                {
                    await _seedingManager.SeedAllAsync(amount, password);
                });

            AnsiConsole.MarkupLine($"[green]Successfully seeded {amount} of each user type[/]");
            PressAnyKey();
        }

        private static (int amount, string password) PromptSeedingInput()
        {
            var amount = AnsiConsole.Ask<int>("Enter [green]amount[/] to seed:");
            
            if (amount <= 0)
            {
                throw new ArgumentException("Amount must be a positive integer.");
            }

            var password = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter [green]password[/] for seeded accounts:")
                    .PromptStyle("green")
                    .Secret());

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be empty.");
            }

            return (amount, password);
        }

        private static void PressAnyKey()
        {
            AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
            Console.ReadKey(true);
        }
    }
}