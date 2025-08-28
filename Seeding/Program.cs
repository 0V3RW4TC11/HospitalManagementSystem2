using Seeding;
using Seeding.Helpers;

var provider = await DatabaseHelper.SetupAndConfigureDatabase();
new Menu(provider).Run();