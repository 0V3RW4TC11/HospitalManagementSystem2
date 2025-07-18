using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Models.Entities;
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
    
    private const string TestSpecName1 = "TestSpec1";
    private const string TestSpecName2 = "TestSpec2";
    
    // Services
    private readonly SqliteInMemDbHelper _dbHelper;
    private readonly IServiceProvider _serviceProvider;

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
        
        // Set service provider
        _serviceProvider = _dbHelper.ServiceProvider;
        
        // Seed DOCTOR role
        SeedDoctorRole(_serviceProvider);
        
        // Seed Specializations
        SeedSpecializations(_serviceProvider);
    }

    public void Dispose()
    {
        _dbHelper.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _dbHelper.DisposeAsync();
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
        context.Specializations.Add(new Specialization { Name = TestSpecName1 });
        context.Specializations.Add(new Specialization { Name = TestSpecName2 });
        context.SaveChanges();
    }

    private ApplicationDbContext GetContext()
    {
        return _serviceProvider.GetRequiredService<ApplicationDbContext>();
    }
    
    private DoctorService GetSut()
    {
        return _serviceProvider.GetRequiredService<DoctorService>();
    }

    private Specialization GetTestSpec1()
    {
        var context = GetContext();
        return context.Specializations.First(s => s.Name == TestSpecName1);
    }

    private Specialization GetTestSpec2()
    {
        var context = GetContext();
        return context.Specializations.First(s => s.Name == TestSpecName2);
    }

    private string GetDoctorRoleId()
    {
        var context = _serviceProvider.GetRequiredService<ApplicationDbContext>();
        return context.Roles.First(r => r.Name == Constants.AuthRoles.Doctor).Id;
    }
    
    private Doctor CreateDoctor(IEnumerable<Specialization>? specializations = null)
    {
        var doctor = new Doctor
        {
            FirstName = TestFirstName,
            LastName = TestLastName,
            Gender = TestGender,
            Address = TestAddress,
            Phone = TestPhone,
            Email = TestEmail,
            DateOfBirth = TestDateOfBirth
        };
        
        if (specializations != null)
        {
            doctor.Specializations = specializations;
        }
        
        return doctor;
    }

    private Doctor CreateAndSeedDoctor(IEnumerable<Specialization>? specializations = null)
    {
        // Get context
        var context = GetContext();
        // Materialize enumerable if any
        specializations = specializations?.ToArray();
        // Create Doctor with specs if any
        var doctor = CreateDoctor(specializations);
        
        // Add Doctor to database
        context.Doctors.Add(doctor);
        context.SaveChanges();
        
        // Create IdentityUser for Doctor
        var userMan = _serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var identityUser = new IdentityUser { UserName = ExpectedOrgEmail };
        userMan.CreateAsync(identityUser, TestPassword).GetAwaiter().GetResult();
        
        // Add IdentityUser to DOCTOR role
        userMan.AddToRoleAsync(identityUser, Constants.AuthRoles.Doctor).GetAwaiter().GetResult();
        
        // Create Account for Doctor and IdentityUser
        context.Accounts.Add(new Account
        {
            UserId = doctor.Id,
            IdentityUserId = identityUser.Id
        });
        context.SaveChanges();
        
        // If no specs, return the Doctor without specs
        if (specializations == null) return doctor;
        
        // If specs, add DoctorSpecs to database
        foreach (var spec in specializations)
        {
            context.DoctorSpecializations.Add(new DoctorSpecialization
            {
                DoctorId = doctor.Id, 
                SpecializationId = spec.Id
            });
        }
        context.SaveChanges();
        
        // Return Doctor with specs
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
        Assert.Contains(context.Specializations, s => s.Name == TestSpecName1);
        Assert.Contains(context.Specializations, s => s.Name == TestSpecName2);
    }

    [Fact]
    public async Task CreateAsync_NewDoctor_AddsDoctorAccountDoctorSpec()
    {
        // Arrange
        var context = GetContext();
        var sut = GetSut();
        var spec1 = GetTestSpec1();
        var doctor = CreateDoctor([spec1]);
        
        // Act
        await sut.CreateAsync(doctor, TestPassword);
        
        // Assert
        // Doctor
        Assert.Single(context.Doctors);
        var doctorResult = context.Doctors.FirstOrDefault(d => d.Id == doctor.Id);
        Assert.NotNull(doctorResult);
        Assert.Equal(doctor.FirstName, doctorResult.FirstName);
        Assert.Equal(doctor.LastName, doctorResult.LastName);
        Assert.Equal(doctor.Gender, doctorResult.Gender);
        Assert.Equal(doctor.Address, doctorResult.Address);
        Assert.Equal(doctor.Phone, doctorResult.Phone);
        Assert.Equal(doctor.Email, doctorResult.Email);
        Assert.Equal(doctor.DateOfBirth, doctorResult.DateOfBirth);
        
        // Doctor Specialization
        Assert.Single(context.DoctorSpecializations);
        Assert.Contains(context.DoctorSpecializations, 
            ds => ds.DoctorId == doctor.Id && ds.SpecializationId == spec1.Id);
        
        // Account
        Assert.Single(context.Accounts);
        var account = context.Accounts.FirstOrDefault(a => a.UserId == doctor.Id);
        Assert.NotNull(account);
        
        // IdentityUser
        Assert.Single(context.Users);
        Assert.Contains(context.Users, u => u.Id == account.IdentityUserId);
        Assert.Single(context.UserRoles);
        var userRole = context.UserRoles.FirstOrDefault(ur => ur.UserId == account.IdentityUserId);
        Assert.NotNull(userRole);
        Assert.Equal(GetDoctorRoleId(), userRole.RoleId);
    }

    [Fact]
    public async Task CreateAsync_NullDoctor_Throws()
    {
        // Arrange
        var context = GetContext();
        var sut = GetSut();
        
        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() => sut.CreateAsync(null, TestPassword));
        Assert.Empty(context.Doctors);
        Assert.Empty(context.DoctorSpecializations);
        Assert.Empty(context.Accounts);
        Assert.Empty(context.Users);
        Assert.Empty(context.UserRoles);
    }

    [Fact]
    public async Task CreateAsync_NewDoctorEmptyPassword_Throws()
    {
        // Arrange
        var context = GetContext();
        var sut = GetSut();
        var spec1 = GetTestSpec1();
        var doctor = CreateDoctor([spec1]);
        
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => sut.CreateAsync(doctor, string.Empty));
        Assert.Contains("Passwords must be", result.Message);
    }

    [Fact]
    public async Task CreateAsync_NewDoctorMissingDetails_Throws()
    {
        // Arrange
        var context = GetContext();
        var sut = GetSut();
        var spec1 = GetTestSpec1();
        var doctor = CreateDoctor([spec1]);
        doctor.FirstName = null;
        
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => sut.CreateAsync(doctor, TestPassword));
        Assert.Contains("An error occurred while saving the entity", result.Message);
    }

    [Fact]
    public async Task CreateAsync_ExistingDoctor_Throws()
    {
        // Arrange
        var context = GetContext();
        var sut = GetSut();
        var spec1 = GetTestSpec1();
        CreateAndSeedDoctor([spec1]);
        var duplicate = CreateDoctor([spec1]);
        
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => sut.CreateAsync(duplicate, TestPassword));
        Assert.Equal("A duplicate record exists", result.Message);
        Assert.Single(context.Doctors);
        Assert.Single(context.DoctorSpecializations);
        Assert.Single(context.Accounts);
        Assert.Single(context.Users);
        Assert.Single(context.UserRoles);
    }
    
    [Fact]
    public async Task FindByIdAsync_ExistingDoctor_ReturnsDoctor()
    {
        // Arrange
        var sut = GetSut();
        var spec1 = GetTestSpec1();
        var doctor = CreateAndSeedDoctor([spec1]);
        
        // Act
        var result = await sut.FindByIdAsync(doctor.Id);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(doctor.Id, result.Id);
        Assert.Equal(doctor.FirstName, result.FirstName);
        Assert.Equal(doctor.LastName, result.LastName);
        Assert.Equal(doctor.Gender, result.Gender);
        Assert.Equal(doctor.Address, result.Address);
        Assert.Equal(doctor.Phone, result.Phone);
        Assert.Equal(doctor.Email, result.Email);
        Assert.Equal(doctor.DateOfBirth, result.DateOfBirth);
        Assert.NotEmpty(doctor.Specializations);
        Assert.Contains(doctor.Specializations, s => s.Id == spec1.Id);
    }

    [Fact]
    public async Task FindByIdAsync_ExistingDoctorNoSpecs_ReturnsDoctorWithNoSpecs()
    {
        // Arrange
        var sut = GetSut();
        var doctor = CreateAndSeedDoctor();
        
        // Act
        var result = await sut.FindByIdAsync(doctor.Id);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(doctor.Id, result.Id);
        Assert.Empty(result.Specializations);
    }

    [Fact]
    public async Task FindByIdAsync_NonExistingDoctor_ReturnsNull()
    {
        // Arrange
        var sut = GetSut();
        
        // Act
        var result = await sut.FindByIdAsync(Guid.NewGuid());
        
        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task FindByIdAsync_EmptyId_ReturnsNull()
    {
        // Arrange
        var sut = GetSut();
        
        // Act
        var result = await sut.FindByIdAsync(Guid.Empty);
        
        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task UpdateAsync_ExistingDoctor_UpdatesDoctor()
    {
        // Arrange
        var context = GetContext();
        var sut = GetSut();
        var spec1 = GetTestSpec1();
        var spec2 = GetTestSpec2();
        var doctor = CreateAndSeedDoctor([spec1]);
        
        doctor.FirstName = "UpdatedFirstName";
        doctor.LastName = "UpdatedLastName";
        doctor.Gender = "UpdatedGender";
        doctor.Address = "UpdatedAddress";
        doctor.Phone = "UpdatedPhone";
        doctor.Email = "UpdatedEmail";
        doctor.DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow);
        var docSpecs = doctor.Specializations.ToList();
        docSpecs.Add(spec2);
        doctor.Specializations = docSpecs;

        // Act
        await sut.UpdateAsync(doctor);
        
        // Assert
        var result = context.Doctors.First(d => d.Id == doctor.Id);
        Assert.Equal(doctor.FirstName, result.FirstName);
        Assert.Equal(doctor.LastName, result.LastName);
        Assert.Equal(doctor.Gender, result.Gender);
        Assert.Equal(doctor.Address, result.Address);
        Assert.Equal(doctor.Phone, result.Phone);
        Assert.Equal(doctor.Email, result.Email);
        Assert.Equal(doctor.DateOfBirth, result.DateOfBirth);

        var resultSpecs = context.DoctorSpecializations.Where(ds => ds.DoctorId == doctor.Id);
        Assert.Equal(doctor.Specializations.Count(), resultSpecs.Count());
        Assert.Contains(resultSpecs, rs => rs.SpecializationId == spec1.Id);
        Assert.Contains(resultSpecs, rs => rs.SpecializationId == spec2.Id);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingDoctor_Throws()
    {
        // Arrange
        var sut = GetSut();
        var spec1 = GetTestSpec1();
        var doctor = CreateDoctor([spec1]);
        doctor.Id = Guid.NewGuid();
        
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => sut.UpdateAsync(doctor));
        Assert.Contains("Sequence contains no elements", result.Message);
    }
    
    [Fact]
    public async Task UpdateAsync_ExistingDoctorMissingDetails_Throws()
    {
        // Arrange
        var sut = GetSut();
        var spec1 = GetTestSpec1();
        var doctor = CreateAndSeedDoctor([spec1]);
        
        doctor.FirstName = null;
        doctor.LastName = null;
        
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => sut.UpdateAsync(doctor));
        Assert.Contains("An error occurred while saving the entity changes", result.Message);
    }
    
    [Fact]
    public async Task UpdateAsync_ExistingDoctorMissingSpecializations_Throws()
    {
        // Arrange
        var sut = GetSut();
        var spec1 = GetTestSpec1();
        var doctor = CreateAndSeedDoctor([spec1]);
        
        var specs = doctor.Specializations.ToList();
        specs.Clear();
        doctor.Specializations = specs;
        
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => sut.UpdateAsync(doctor));
        Assert.Equal("Specialization list cannot be empty", result.Message);
    }
    
    [Fact]
    public async Task UpdateAsync_NullDoctor_Throws()
    {
        // Arrange
        var sut = GetSut();
        
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => sut.UpdateAsync(null));
        Assert.Contains("Object reference not set to an instance of an object", result.Message);
    }

    [Fact]
    public async Task DeleteAsync_ExistingDoctor_DeletesDoctorDoctorSpecsAccount()
    {
        // Arrange
        var context = GetContext();
        var sut = GetSut();
        var spec1 = GetTestSpec1();
        var spec2 = GetTestSpec2();
        var doctor = CreateAndSeedDoctor([spec1, spec2]);
        
        // Act
        await sut.DeleteAsync(doctor);
        
        // Assert
        Assert.Empty(context.Doctors);
        Assert.Empty(context.DoctorSpecializations);
        Assert.Empty(context.Accounts);
        Assert.Empty(context.Users);
        Assert.Empty(context.UserRoles);
    }
    
    [Fact]
    public async Task DeleteAsync_NonExistingDoctor_Throws()
    {
        // Arrange
        var sut = GetSut();
        var doctor = CreateDoctor();
        doctor.Id = Guid.NewGuid();
        
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => sut.DeleteAsync(doctor));
        Assert.Contains("Sequence contains no elements", result.Message);
    }

    [Fact]
    public async Task DeleteAsync_NullDoctor_Throws()
    {
        // Arrange
        var sut = GetSut();
        
        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => sut.DeleteAsync(null!));
        Assert.Contains("Object reference not set to an instance of an object", result.Message);
    }
}