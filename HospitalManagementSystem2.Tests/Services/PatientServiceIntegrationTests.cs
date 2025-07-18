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

    private Patient CreateAndSeedPatientAccount()
    {
        var context = GetContext();
        var patient = PatientTestData.CreateAndSeedPatient(context);
        TestAccountHelper.SeedAccount(context, 
                                      _dbHelper.ServiceProvider, 
                                      patient.Id, 
                                      patient.Email, 
                                      PatientTestData.TestPassword, 
                                      Constants.AuthRoles.Patient);

        return patient;
    }

    [Fact]
    public async Task CreateAsync_NewPatient_AddsPatientAndAccount()
    {
        // Arrange
        var context = GetContext();
        var patient = PatientTestData.CreatePatient();
        
        // Act
        await GetSut().CreateAsync(patient, PatientTestData.TestPassword);
        
        // Assert
        var patientResult = context.Patients.FirstOrDefault(p => p.Id == patient.Id);
        Assert.NotNull(patientResult);
        Assert.Equal(PatientTestData.Title, patientResult.Title);
        Assert.Equal(PatientTestData.FirstName, patientResult.FirstName);
        Assert.Equal(PatientTestData.LastName, patientResult.LastName);
        Assert.Equal(PatientTestData.Gender, patientResult.Gender);
        Assert.Equal(PatientTestData.Address, patientResult.Address);
        Assert.Equal(PatientTestData.Phone, patientResult.Phone);
        Assert.Equal(PatientTestData.Email, patientResult.Email);
        Assert.Equal(PatientTestData.DateOfBirth, patientResult.DateOfBirth);
        Assert.Equal(PatientTestData.BloodType, patientResult.BloodType);
        
        TestAccountHelper.AssertHasAccount(context, patient.Id, GetPatientRoleId());
    }
    
    [Fact]
    public async Task CreateAsync_NewPatientInvalidData_Throws()
    {
        // Arrange
        var patient = PatientTestData.CreatePatient();
        patient.FirstName = null!;
        
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() 
            => GetSut().CreateAsync(patient, PatientTestData.TestPassword));
        Assert.Contains(ErrorMessageHelper.EntityChangesError, result.Message);
    }
    
    [Fact]
    public async Task CreateAsync_ExistingPatient_Throws()
    {
        // Arrange
        var patient = PatientTestData.CreateAndSeedPatient(GetContext());
        
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => GetSut().CreateAsync(patient, PatientTestData.TestPassword));
        Assert.Contains(ErrorMessageHelper.DuplicateRecord, result.Message);
    }
    
    [Fact]
    public async Task CreateAsync_NullPatient_Throws()
    {
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => GetSut().CreateAsync(null, PatientTestData.TestPassword));
        Assert.Contains(ErrorMessageHelper.LinqQueryExceptionThrown, result.Message);
    }
    
    [Fact]
    public async Task FindByIdAsync_ExistingPatient_ReturnsPatient()
    {
        // Arrange
        var patient = CreateAndSeedPatientAccount();
        
        // Act
        var result = await GetSut().FindByIdAsync(patient.Id);
        
        // Assert
        Assert.NotNull(result);
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
        var patient = CreateAndSeedPatientAccount();
        patient.FirstName = "NewFirstName";
        patient.BloodType = BloodType.OPositive;
        
        // Act
        await GetSut().UpdateAsync(patient);
        
        // Assert
        var patientResult = context.Patients.FirstOrDefault(p => p.Id == patient.Id);
        Assert.NotNull(patientResult);
        Assert.Equal(patient.FirstName, patientResult.FirstName);
        Assert.Equal(patient.BloodType, patientResult.BloodType);
    }
    
    [Fact]
    public async Task UpdateAsync_ExistingPatientInvalidData_UpdatesPatient()
    {
        // Arrange
        var patient = CreateAndSeedPatientAccount();
        patient.FirstName = null!;
        
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => GetSut().UpdateAsync(patient));
        Assert.Contains(ErrorMessageHelper.EntityChangesError, result.Message);
    }
    
    [Fact]
    public async Task UpdateAsync_NewPatient_Throws()
    {
        // Arrange
        var patient = PatientTestData.CreatePatient();
        
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => GetSut().UpdateAsync(patient));
        Assert.Contains(ErrorMessageHelper.SequenceNoElements, result.Message);
    }
    
    [Fact]
    public async Task UpdateAsync_NullPatient_Throws()
    {
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => GetSut().UpdateAsync(null!));
        Assert.Contains(ErrorMessageHelper.LinqQueryExceptionThrown, result.Message);
    }
    
    [Fact]
    public async Task DeleteAsync_ExistingPatient_DeletesPatientAccount()
    {
        // Arrange
        var context = GetContext();
        var patient = CreateAndSeedPatientAccount();
        
        // Act
        await GetSut().DeleteAsync(patient);
        
        // Assert
        Assert.Empty(context.Patients);
        Assert.Empty(context.Accounts);
        Assert.Empty(context.Users);
        Assert.Empty(context.UserRoles);
    }
    
    [Fact]
    public async Task DeleteAsync_NewPatient_Throws()
    {
        // Arrange
        var patient = PatientTestData.CreatePatient();
        
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => GetSut().DeleteAsync(patient));
        Assert.Contains(ErrorMessageHelper.SequenceNoElements, result.Message);
    }
    
    [Fact]
    public async Task DeleteAsync_NullPatient_Throws()
    {
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => GetSut().DeleteAsync(null!));
        Assert.Contains(ErrorMessageHelper.LinqQueryExceptionThrown, result.Message);
    }
}