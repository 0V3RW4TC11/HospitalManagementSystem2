using Seeding;
using Seeding.Helpers;

var services = await DatabaseHelper.SetupAndConfigureDatabase();
var menuController = new MenuController(services);
await menuController.RunMenuAsync();