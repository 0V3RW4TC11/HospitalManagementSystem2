using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Models.Entities;
using HospitalManagementSystem2.Repositories;
using HospitalManagementSystem2.Services;
using HospitalManagementSystem2.Tests.Helpers;
using HospitalManagementSystem2.Tests.TestData;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace HospitalManagementSystem2.Tests.Services;

public class PatientServiceIntegrationTests : IDisposable, IAsyncDisposable
{
    private readonly SqliteInMemDbHelper _dbHelper;

    public PatientServiceIntegrationTests()
    {
        _dbHelper = new SqliteInMemDbHelper(services =>
        {
            services.AddScoped<IDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IPatientRepository, PatientRepository>();
            services.AddScoped<AccountService>();
            services.AddScoped<PatientService>();
        });
        
        SeedPatientRole();
    }

    public void Dispose()
    {
        _dbHelper.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _dbHelper.DisposeAsync();
    }

    private void SeedPatientRole()
    {
        using var scope = _dbHelper.ServiceProvider.CreateScope();
        var roleMan = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        roleMan.CreateAsync(new IdentityRole(Constants.AuthRoles.Patient)).GetAwaiter().GetResult();
    }
    
    private ApplicationDbContext GetContext() => _dbHelper.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    private PatientService GetSut() => _dbHelper.ServiceProvider.GetRequiredService<PatientService>();

    private string GetPatientRoleId()
    {
        var context = GetContext();
        return context.Roles.First(r => r.Name == Constants.AuthRoles.Patient).Id;
    }

    private Patient CreatePatientAccount()
    {
        var context = GetContext();
        var patient = PatientTestHelper.CreateAndSeedPatient(context);
        AccountTestHelper.SeedAccount(context, 
                                      _dbHelper.ServiceProvider, 
                                      patient.Id, 
                                      patient.Email, 
                                      PersonTestData.TestPassword, 
                                      Constants.AuthRoles.Patient);

        return patient;
    }

    [Fact]
    public async Task CreateAsync_NewPatient_AddsPatientAndAccount()
    {
        // Arrange
        var context = GetContext();
        var patient = PatientTestHelper.CreatePatient();
        
        // Act
        await GetSut().CreateAsync(patient, PersonTestData.TestPassword);
        
        // Assert
        PatientTestHelper.AssertHasData(context, patient);
        AccountTestHelper.AssertHasAccount(context, patient.Id, GetPatientRoleId());
    }
    
    [Fact]
    public async Task CreateAsync_NewPatientInvalidData_Throws()
    {
        // Arrange
        var patient = PatientTestHelper.CreatePatient();
        patient.FirstName = null!;
        
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => GetSut().CreateAsync(patient, PersonTestData.TestPassword));
        Assert.Contains(ErrorMessageData.EntityChangesError, result.Message);
    }
    
    [Fact]
    public async Task CreateAsync_NewPatientExistingData_Throws()
    {
        // Arrange
        var patient = PatientTestHelper.CreateAndSeedPatient(GetContext());
        
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => GetSut().CreateAsync(patient, PersonTestData.TestPassword));
        Assert.Contains(ErrorMessageData.DuplicateRecord, result.Message);
    }
    
    [Fact]
    public async Task CreateAsync_NullPatient_Throws()
    {
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => GetSut().CreateAsync(null, PersonTestData.TestPassword));
        Assert.Contains(ErrorMessageData.LinqQueryExceptionThrown, result.Message);
    }
    
    [Fact]
    public async Task FindByIdAsync_ExistingPatient_ReturnsPatient()
    {
        // Arrange
        var patient = CreatePatientAccount();
        
        // Act
        var result = await GetSut().FindByIdAsync(patient.Id);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equivalent(patient, result, true);
    }
    
    [Fact]
    public async Task FindByIdAsync_NonExistingPatient_ReturnsNull()
    {
        // Act
        var result = await GetSut().FindByIdAsync(Guid.NewGuid());
        
        // Assert
        Assert.Null(result);   
    }
    
    [Fact]
    public async Task FindByIdAsync_EmptyGuid_ReturnsNull()
    {
        // Act
        var result = await GetSut().FindByIdAsync(Guid.Empty);
        
        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task UpdateAsync_ExistingPatientNewData_UpdatesPatient()
    {
        // Arrange
        var context = GetContext();
        var patient = CreatePatientAccount();
        patient.FirstName = "NewFirstName";
        patient.BloodType = BloodType.OPositive;
        
        // Act
        await GetSut().UpdateAsync(patient);
        
        // Assert
        PatientTestHelper.AssertHasData(context, patient);
    }
    
    [Fact]
    public async Task UpdateAsync_ExistingPatientInvalidData_UpdatesPatient()
    {
        // Arrange
        var patient = CreatePatientAccount();
        patient.FirstName = null!;
        
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => GetSut().UpdateAsync(patient));
        Assert.Contains(ErrorMessageData.EntityChangesError, result.Message);
    }
    
    [Fact]
    public async Task UpdateAsync_NewPatient_Throws()
    {
        // Arrange
        var patient = PatientTestHelper.CreatePatient();
        
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => GetSut().UpdateAsync(patient));
        Assert.Contains(ErrorMessageData.SequenceNoElements, result.Message);
    }
    
    [Fact]
    public async Task UpdateAsync_NullPatient_Throws()
    {
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => GetSut().UpdateAsync(null!));
        Assert.Contains(ErrorMessageData.LinqQueryExceptionThrown, result.Message);
    }
    
    [Fact]
    public async Task DeleteAsync_ExistingPatient_DeletesPatientAccount()
    {
        // Arrange
        var context = GetContext();
        var patient = CreatePatientAccount();
        var account = AccountTestHelper.GetAccount(context, patient.Id);
        
        // Act
        await GetSut().DeleteAsync(patient);
        
        // Assert
        PatientTestHelper.AssertHasNoData(context, patient);
        AccountTestHelper.AssertHasNoAccount(context, account);
    }
    
    [Fact]
    public async Task DeleteAsync_NewPatient_Throws()
    {
        // Arrange
        var patient = PatientTestHelper.CreatePatient();
        
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => GetSut().DeleteAsync(patient));
        Assert.Contains(ErrorMessageData.SequenceNoElements, result.Message);
    }
    
    [Fact]
    public async Task DeleteAsync_NullPatient_Throws()
    {
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => GetSut().DeleteAsync(null!));
        Assert.Contains(ErrorMessageData.LinqQueryExceptionThrown, result.Message);
    }
}