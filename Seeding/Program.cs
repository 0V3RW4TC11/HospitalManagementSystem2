using Microsoft.Extensions.DependencyInjection;
using Persistence;
using Seeding.Helpers;
using Seeding.Seeders;

const string password = "Pass123!";
var services = await DatabaseHelper.SetupAndConfigureDatabase();

//var role = await IdentityRoleHelper.GetRoleAsync(services, Constants.AuthRoles.Admin);
//var seeder = new AdminSeeder(services, role.Id, password);

//var role = await IdentityRoleHelper.GetRoleAsync(services, Constants.AuthRoles.Patient);
//var seeder = new PatientSeeder(services, role.Id, password);

var role = await IdentityRoleHelper.GetRoleAsync(services, Constants.AuthRoles.Doctor);
var specIds = await SpecializationsSeeder.SeedAsync(services.GetRequiredService<RepositoryDbContext>(), Path.Combine(AppContext.BaseDirectory, "Data", "specializations.csv"));
var seeder = new DoctorSeeder(services, specIds, role.Id, password);

var time = DateTime.Now;
await seeder.SeedAsync(1000);
var duration = DateTime.Now - time;
Console.WriteLine($"Seeding completed in {duration.TotalSeconds} seconds.");

//new Menu(provider).Run();