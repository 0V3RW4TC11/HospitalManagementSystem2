using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Models.Entities;
using HospitalManagementSystem2.Services;
using HospitalManagementSystem2.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace HospitalManagementSystem2.Tests.Old
{
    public class AdminTestsOld : IClassFixture<DbTestFixture>, IAsyncDisposable
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly IAdminRepository _adminRepository;
        private readonly Func<Task> _resetDatabaseAsync;
        private readonly Admin _admin = new()
        {
            Title = "TestTitle",
            FirstName = "TestFirstName",
            LastName = "TestLastName",
            Gender = "Test",
            Address = "1 Test Test, Test 0000",
            Phone = "0-0-0000-0000",
            Email = "test@example.com",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now)
        };
        private readonly string _adminPassword = "Password123!";

        public AdminTestsOld(DbTestFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
            
            _adminRepository = new AdminRepository(
                _serviceProvider.GetRequiredService<ApplicationDbContext>(),
                _serviceProvider.GetRequiredService<AccountHelper>(),
                new StaffEmailGenerator(_serviceProvider.GetRequiredService<UserManager<IdentityUser>>()));

            _resetDatabaseAsync = fixture.ResetDatabaseAsync;
            
            SeedAdminRoleAsync().GetAwaiter().GetResult();
        }

        [Fact]
        public async Task AddAdminToDatabase()
        {
            // Arrange
            var userManager = _serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var admin = _admin.ShallowClone();

            // Act
            await _adminRepository.CreateAsync(admin, _adminPassword);

            // Assert
            Assert.NotEqual(Guid.Empty, admin.Id);
            Assert.NotNull(userManager.FindByIdAsync(admin.UserId));
            Assert.True(await userManager.IsInRoleAsync(admin.User, Constants.AuthRoles.Admin));
        }

        public async ValueTask DisposeAsync()
        {
            await _resetDatabaseAsync();
            await SeedAdminRoleAsync();
        }

        //[Fact]
        //public async Task GetAdminFromDatabase()
        //{

        //}

        //[Fact] 
        //async Task UpdateAdminToDatabase()
        //{

        //}

        //[Fact]
        //async Task DeleteAdminFromDatabase()
        //{

        //}

        private async Task SeedAdminRoleAsync()
        {
            var roleManager = _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var result = await roleManager.CreateAsync(new(Constants.AuthRoles.Admin));
            AccountHelper.CheckIdentityResult(result);
        }
    }
}
