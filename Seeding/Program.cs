using Seeding;
using Seeding.Helpers;

var dbManager = new DatabaseManager();
await dbManager.InitializeDatabase();

var menuController = new MenuController(dbManager.Services);
await menuController.RunMenuAsync();