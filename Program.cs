using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Repositories;
using HospitalManagementSystem2.Services;
using HospitalManagementSystem2.Tests;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------
// Services
// ------------------------------------------------

// Database //

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddControllersWithViews();

// Identity //

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();

// Scoped services //

builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IPersonRepository, PersonRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRespository>();
builder.Services.AddScoped<AccountManager>();
builder.Services.AddScoped<AdminManager>();

var app = builder.Build();

// ------------------------------------------------
// Pipelines
// ------------------------------------------------

// HTTP Request //

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// Identity //

app.UseAuthentication();
app.UseAuthorization();

// ------------------------------------------------
// Pre-launch routines
// ------------------------------------------------

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var accountManager = scope.ServiceProvider.GetRequiredService<AccountManager>();
    var adminManager = scope.ServiceProvider.GetRequiredService<AdminManager>();

    var seeding = new Seeding(roleManager);
    var adminTest = new AdminTests(accountManager, adminManager);

    await seeding.SeedRoles();
}

// ------------------------------------------------
// App launch
// ------------------------------------------------

app.Run();
