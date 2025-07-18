using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Repositories;
using HospitalManagementSystem2.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------
// Services
// ------------------------------------------------

// Database //

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddControllersWithViews();

// Identity //

builder.Services
    .AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();

// Scoped services //

// builder.Services.AddScoped<IDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
// builder.Services.AddScoped<IStaffEmailGenerator, StaffEmailGenerator>();
// builder.Services.AddScoped<IAccountRepository, AccountRepository>();
// builder.Services.AddScoped<IAdminRepository, AdminRepository>();
// builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
// builder.Services.AddScoped<IDoctorSpecializationRepository, DoctorSpecializationRepository>();
// builder.Services.AddScoped<ISpecializationRepository, SpecializationRepository>();
// builder.Services.AddScoped<AccountService>();

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
    "default",
    "{controller=Home}/{action=Index}/{id?}");
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
    var seeding = new Seeding(roleManager);
    await seeding.SeedRolesAsync();
}

// ------------------------------------------------
// App launch
// ------------------------------------------------

app.Run();