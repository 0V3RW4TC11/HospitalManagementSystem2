using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Repositories;
using HospitalManagementSystem2.Services;
using HospitalManagementSystem2.Tests.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace HospitalManagementSystem2.Tests.Services;

public class DoctorServiceIntegrationTests : IDisposable, IAsyncDisposable
{
    // Test Details
    private const string TestFirstName = "TestFirstName";
    private const string TestLastName = "TestLastName";
    private const string TestGender = "TestGender";
    private const string TestAddress = "TestAddress";
    private const string TestPhone = "TestPhone";
    private const string TestEmail = "TestEmail";
    private const string TestPassword = "TestPass123!";
    private static readonly string ExpectedOrgEmail 
        = $"{TestFirstName.ToLower()}.{TestLastName.ToLower()}@{Constants.StaffEmailDomain}";

    private static readonly DateOnly TestDateOfBirth = DateOnly.FromDateTime(DateTime.UnixEpoch);
    
    // Services
    private readonly SqliteInMemDbHelper _dbHelper;
    private readonly IServiceProvider _serviceProvider;

    public DoctorServiceIntegrationTests()
    {
        _dbHelper = new SqliteInMemDbHelper(services =>
        {
            services.AddScoped<IDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<AccountService>();
            services.AddScoped<IDoctorRepository, DoctorRepository>();
            services.AddScoped<IStaffEmailGenerator, StaffEmailGenerator>();
            services.AddScoped<DoctorService>();
        });
        
        // Set service provider
        _serviceProvider = _dbHelper.ServiceProvider;
        
        // Inject DOCTOR role
        using var scope = _serviceProvider.CreateScope();
        var roleMan = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        roleMan.CreateAsync(new IdentityRole(Constants.AuthRoles.Doctor)).GetAwaiter().GetResult();
    }

    public void Dispose()
    {
        _dbHelper.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _dbHelper.DisposeAsync();
    }

    [Fact]
    public async Task CreateAsync_NewDoctor_AddsNewDoctorAndAccount()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task CreateAsync_NullDoctor_Throws()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task CreateAsync_NewDoctorEmptyPassowrd_Throws()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task CreateAsync_NewDoctorMissingDetails_Throws()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task CreateAsync_ExistingDoctor_Throws()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public async Task FindByIdAsync_ExistingDoctor_ReturnsDoctor()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task FindByIdAsync_ExistingDoctorNoSpecializations_Throws()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task FindByIdAsync_NonExistingDoctor_ReturnsNull()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public async Task FindByIdAsync_EmptyId_Throws()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public async Task UpdateAsync_ExistingDoctor_UpdatesDoctor()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task UpdateAsync_NonExistingDoctor_Throws()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public async Task UpdateAsync_ExistingDoctorMissingDetails_Throws()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public async Task UpdateAsync_ExistingDoctorMissingSpecializations_Throws()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public async Task UpdateAsync_NullDoctor_Throws()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task DeleteAsync_ExistingDoctor_DeletesDoctorAndAccount()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public async Task DeleteAsync_NonExistingDoctor_Throws()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task DeleteAsync_NullDoctor_Throws()
    {
        throw new NotImplementedException();
    }
}