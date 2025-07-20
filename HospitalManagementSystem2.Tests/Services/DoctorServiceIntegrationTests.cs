using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Models.Entities;
using HospitalManagementSystem2.Repositories;
using HospitalManagementSystem2.Services;
using HospitalManagementSystem2.Tests.Helpers;
using HospitalManagementSystem2.Tests.TestData;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HospitalManagementSystem2.Tests.Services;

public class DoctorServiceIntegrationTests : IDisposable, IAsyncDisposable
{
    // Services
    private readonly SqliteInMemDbHelper _dbHelper;

    public DoctorServiceIntegrationTests()
    {
        _dbHelper = new SqliteInMemDbHelper(services =>
        {
            services.AddScoped<IDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IDoctorRepository, DoctorRepository>();
            services.AddScoped<IDoctorSpecializationRepository, DoctorSpecializationRepository>();
            services.AddScoped<ISpecializationRepository, SpecializationRepository>();
            services.AddScoped<IStaffEmailGenerator, StaffEmailGenerator>();
            services.AddScoped<AccountService>();
            services.AddScoped<DoctorService>();
        });

        // Seed DOCTOR role
        SeedDoctorRole(_dbHelper.ServiceProvider);

        // Seed Specializations
        SeedSpecializations(_dbHelper.ServiceProvider);
    }

    public async ValueTask DisposeAsync()
    {
        await _dbHelper.DisposeAsync();
    }

    public void Dispose()
    {
        _dbHelper.Dispose();
    }

    private void SeedDoctorRole(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleMan = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        roleMan.CreateAsync(new IdentityRole(Constants.AuthRoles.Doctor)).GetAwaiter().GetResult();
    }

    private void SeedSpecializations(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Specializations.Add(SpecializationTestHelper.CreateTestSpec1());
        context.Specializations.Add(SpecializationTestHelper.CreateTestSpec2());
        context.SaveChanges();
    }

    private ApplicationDbContext GetContext() => _dbHelper.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    private DoctorService GetSut() => _dbHelper.ServiceProvider.GetRequiredService<DoctorService>();

    private string GetDoctorRoleId() => GetContext().Roles.Single(r => r.Name == Constants.AuthRoles.Doctor).Id;

    private Doctor CreateDoctorAccount(IEnumerable<Specialization>? specializations = null)
    {
        var context = GetContext();
        var doctor = DoctorTestHelper.CreateAndSeedDoctor(context, specializations);
        AccountTestHelper.SeedAccount(context, 
                                      _dbHelper.ServiceProvider, 
                                      doctor.Id, 
                                      StaffTestHelper.ExpectedOrgEmail(doctor.FirstName, doctor.LastName), 
                                      PersonTestData.TestPassword, 
                                      Constants.AuthRoles.Doctor);
        return doctor;
    }

    [Fact]
    public void InitializationCheck()
    {
        // Arrange
        var context = GetContext();

        // Act (See constructor)

        // Assert
        Assert.Contains(context.Roles, r => r.Name == Constants.AuthRoles.Doctor);
        Assert.Contains(context.Specializations, s => s.Name == SpecializationTestHelper.TestSpec1Name);
        Assert.Contains(context.Specializations, s => s.Name == SpecializationTestHelper.TestSpec2Name);
    }

    [Fact]
    public async Task CreateAsync_NewDoctor_AddsDoctorData()
    {
        // Arrange
        var context = GetContext();
        var spec1 = SpecializationTestHelper.GetTestSpec1(context);
        var doctor = DoctorTestHelper.CreateDoctor([spec1]);

        // Act
        await GetSut().CreateAsync(doctor, PersonTestData.TestPassword);

        // Assert
        DoctorTestHelper.AssertHasData(context, doctor);
        AccountTestHelper.AssertHasAccount(context, doctor.Id, GetDoctorRoleId());
    }

    [Fact]
    public async Task CreateAsync_NewDoctorExistingData_Throws()
    {
        // Arrange
        var spec1 = SpecializationTestHelper.GetTestSpec1(GetContext());
        CreateDoctorAccount([spec1]);
        var duplicate = DoctorTestHelper.CreateDoctor([spec1]);

        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => GetSut().CreateAsync(duplicate, PersonTestData.TestPassword));
        Assert.Equal(ErrorMessageData.DuplicateRecord, result.Message);
    }

    [Fact]
    public async Task CreateAsync_NewDoctorInvalidData_Throws()
    {
        // Arrange
        var spec1 = SpecializationTestHelper.GetTestSpec1(GetContext());
        var doctor = DoctorTestHelper.CreateDoctor([spec1]);
        doctor.FirstName = null;
        doctor.LastName = null;

        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => GetSut().CreateAsync(doctor, PersonTestData.TestPassword));
        Assert.Contains(ErrorMessageData.EntityChangesError, result.Message);
    }

    [Fact]
    public async Task CreateAsync_NullDoctor_Throws()
    {
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => GetSut().CreateAsync(null!, PersonTestData.TestPassword));
        Assert.Contains(ErrorMessageData.LinqQueryExceptionThrown, result.Message);
    }

    [Fact]
    public async Task FindByIdAsync_ExistingDoctor_ReturnsDoctor()
    {
        // Arrange
        var spec1 = SpecializationTestHelper.GetTestSpec1(GetContext());
        var doctor = CreateDoctorAccount([spec1]);

        // Act
        var result = await GetSut().FindByIdAsync(doctor.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equivalent(doctor, result, true);
    }

    [Fact]
    public async Task FindByIdAsync_ExistingDoctorNoSpecs_ReturnsDoctorWithNoSpecs()
    {
        // Arrange
        var sut = GetSut();
        var doctor = CreateDoctorAccount();

        // Act
        var result = await GetSut().FindByIdAsync(doctor.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equivalent(doctor, result, true);
    }

    [Fact]
    public async Task FindByIdAsync_NonExistingDoctor_ReturnsNull()
    {
        // Act
        var result = await GetSut().FindByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task FindByIdAsync_EmptyId_ReturnsNull()
    {
        // Act
        var result = await GetSut().FindByIdAsync(Guid.Empty);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_ExistingDoctor_UpdatesDoctor()
    {
        // Arrange
        var context = GetContext();
        var spec1 = SpecializationTestHelper.GetTestSpec1(context);
        var spec2 = SpecializationTestHelper.GetTestSpec2(context);
        var doctor = CreateDoctorAccount([spec1]);
        
        var docSpecs = doctor.Specializations.ToList();
        docSpecs.Add(spec2);

        doctor.FirstName = "UpdatedFirstName";
        doctor.LastName = "UpdatedLastName";
        doctor.Specializations = docSpecs;

        // Act
        await GetSut().UpdateAsync(doctor);

        // Assert
        DoctorTestHelper.AssertHasData(context, doctor);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingDoctor_Throws()
    {
        // Arrange
        var spec1 = SpecializationTestHelper.GetTestSpec1(GetContext());
        var doctor = DoctorTestHelper.CreateDoctor([spec1]);
        doctor.Id = Guid.NewGuid();

        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => GetSut().UpdateAsync(doctor));
        Assert.Contains(ErrorMessageData.SequenceNoElements, result.Message);
    }

    [Fact]
    public async Task UpdateAsync_ExistingDoctorInvalidData_Throws()
    {
        // Arrange
        var spec1 = SpecializationTestHelper.GetTestSpec1(GetContext());
        var doctor = CreateDoctorAccount([spec1]);
        doctor.FirstName = null;
        doctor.LastName = null;

        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => GetSut().UpdateAsync(doctor));
        Assert.Contains(ErrorMessageData.EntityChangesError, result.Message);
    }

    [Fact]
    public async Task UpdateAsync_ExistingDoctorNoSpecializations_Throws()
    {
        // Arrange
        var context = GetContext();
        var spec1 = SpecializationTestHelper.GetTestSpec1(context);
        var spec2 = SpecializationTestHelper.GetTestSpec2(context);
        var doctor = CreateDoctorAccount([spec1, spec2]);
        doctor.Specializations = [];

        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => GetSut().UpdateAsync(doctor));
        Assert.Equal("Specialization list cannot be empty", result.Message);
    }

    [Fact]
    public async Task UpdateAsync_NullDoctor_Throws()
    {
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => GetSut().UpdateAsync(null!));
        Assert.Contains(ErrorMessageData.ObjectNotSet, result.Message);
    }

    [Fact]
    public async Task DeleteAsync_ExistingDoctor_DeletesDoctorData()
    {
        // Arrange
        var context = GetContext();
        var spec1 = SpecializationTestHelper.GetTestSpec1(context);
        var spec2 = SpecializationTestHelper.GetTestSpec2(context);
        var doctor = CreateDoctorAccount([spec1, spec2]);
        var account = AccountTestHelper.GetAccount(context, doctor.Id);

        // Act
        await GetSut().DeleteAsync(doctor);

        // Assert
        DoctorTestHelper.AssertHasNoData(context, doctor);
        AccountTestHelper.AssertHasNoAccount(context, account);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingDoctor_Throws()
    {
        // Arrange
        var doctor = DoctorTestHelper.CreateDoctor();
        doctor.Id = Guid.NewGuid();

        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => GetSut().DeleteAsync(doctor));
        Assert.Contains(ErrorMessageData.SequenceNoElements, result.Message);
    }

    [Fact]
    public async Task DeleteAsync_NullDoctor_Throws()
    {
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => GetSut().DeleteAsync(null!));
        Assert.Contains(ErrorMessageData.ObjectNotSet, result.Message);
    }
}