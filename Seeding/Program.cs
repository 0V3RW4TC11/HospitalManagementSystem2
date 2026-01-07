using Menu;
using Seeders.Services;

var seedingService = await SeedingService.CreateAsync();
var menuActions = new MenuActions(seedingService);
var menuController = new MenuController(menuActions);
await menuController.RunMenuAsync(); 