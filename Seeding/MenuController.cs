using Spectre.Console;

namespace Menu
{
    internal class MenuController
    {
        private readonly MenuActions _menuActions;

        public MenuController(MenuActions menuActions)
        {
            _menuActions = menuActions;
        }

        public async Task RunMenuAsync()
        {
            bool exit = false;

            while (!exit)
            {
                try
                {
                    Console.Clear();
                    AnsiConsole.Write(
                        new Rule("[yellow]Database Seeding Menu[/]")
                            .RuleStyle("grey")
                            .LeftJustified());
                    AnsiConsole.WriteLine();

                    var menuOptions = await _menuActions.GetActionsAsync();

                    var choice = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("Select an [green]option[/]:")
                            .PageSize(10)
                            .HighlightStyle(new Style(foreground: Color.Green))
                            .AddChoices(menuOptions.Keys));

                    if (choice == "Exit")
                    {
                        exit = true;
                    }
                    else
                    {
                        Console.Clear();
                        AnsiConsole.Write(
                            new Rule($"[cyan]{choice}[/]")
                                .RuleStyle("grey")
                                .LeftJustified());
                        AnsiConsole.WriteLine();

                        await menuOptions[choice]();
                    }
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
                    AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
                    Console.ReadKey(true);
                }
            }

            AnsiConsole.MarkupLine("[grey]Goodbye![/]");
        }
    }
}