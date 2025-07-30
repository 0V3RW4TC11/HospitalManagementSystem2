using DataTransfer.Patient;
using Domain.Constants;
using Domain.Exceptions;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TestData;

namespace Persistence.Tests;

internal sealed class PatientServiceTests : PersistenceTestBase
{
    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        SeedPatientRole();
    }
    
    private void SeedPatientRole()
    {
        var roleManager = GetServiceProvider().GetRequiredService<RoleManager<IdentityRole>>();
        roleManager.CreateAsync(new IdentityRole(AuthRoles.Patient)).GetAwaiter().GetResult();
    }

    [Test]
    public async Task CreateAsync_PatientWithValidData_CreatesPatientAndAccount()
    {
        // Arrange
        var context = GetDbContext();
        var patientCreateDto = PatientTestData.CreateDto();

        // Act
        await GetServiceManager().PatientService.CreateAsync(patientCreateDto);

        // Assert
        // Has unique Patient record
        var patient = context.Patients.Single(p => 
            p.FirstName == patientCreateDto.FirstName &&
            p.LastName == patientCreateDto.LastName &&
            p.Gender == patientCreateDto.Gender &&
            p.Address == patientCreateDto.Address &&
            p.Phone == patientCreateDto.Phone &&
            p.Email == patientCreateDto.Email &&
            p.DateOfBirth == patientCreateDto.DateOfBirth);
        
        // Has unique Account record
        var account = context.Accounts.Single(a => a.UserId == patient.Id);
        
        // Has unique Identity record
        var identityUser = context.Users.Single(a => a.Id == account.IdentityUserId);
        
        // Has expected username
        Assert.That(identityUser.UserName, Is.EqualTo(patientCreateDto.Email));
        
        // Has expected AuthRole
        var patientRoleId = context.Roles.Single(r => r.Name == AuthRoles.Patient).Id;
        Assert.That(context.UserRoles.Any(r => 
            r.UserId == identityUser.Id && 
            r.RoleId == patientRoleId));
    }
    
    [Test]
    public void CreateAsync_PatientWithInvalidData_Throws()
    {
        // Arrange
        var patientCreateDto = PatientTestData.CreateDto();
        patientCreateDto.FirstName = string.Empty;
        patientCreateDto.Gender = string.Empty;
        patientCreateDto.Email = string.Empty;
        
        // Act & Assert
        Assert.ThrowsAsync<PatientBadRequestException>(async () => 
            await GetServiceManager().PatientService.CreateAsync(patientCreateDto));
    }
    
    [Test]
    public async Task CreateAsync_PatientWithDuplicateData_Throws()
    {
        // Arrange
        await PatientTestData.SeedPatient(GetDbContext(), GetIdentityUserManager());
        var patientCreateDto = PatientTestData.CreateDto();
        
        // Act & Assert
        var result = Assert.ThrowsAsync<Exception>(() => 
            GetServiceManager().PatientService.CreateAsync(patientCreateDto));
        Assert.That(
            result.Message, 
            Is.EqualTo($"Username '{patientCreateDto.Email}' is already taken."));
    }
    
    [Test]
    public async Task GetByIdAsync_ExistingPatient_ReturnsPatient()
    {
        // Arrange
        var seededPatientDto = await PatientTestData.SeedPatient(GetDbContext(), GetIdentityUserManager());
        
        // Act
        var patientDto = await GetServiceManager().PatientService.GetByIdAsync(seededPatientDto.Id);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(patientDto.FirstName, Is.EqualTo(seededPatientDto.FirstName));
            Assert.That(patientDto.LastName, Is.EqualTo(seededPatientDto.LastName));
            Assert.That(patientDto.Gender, Is.EqualTo(seededPatientDto.Gender));
            Assert.That(patientDto.Address, Is.EqualTo(seededPatientDto.Address));
            Assert.That(patientDto.Phone, Is.EqualTo(seededPatientDto.Phone));
            Assert.That(patientDto.Email, Is.EqualTo(seededPatientDto.Email));
            Assert.That(patientDto.DateOfBirth, Is.EqualTo(seededPatientDto.DateOfBirth));
        });
    }
    
    [Test]
    public void GetByIdAsync_NonExistingPatient_Throws()
    {
        // Act & Assert
        Assert.ThrowsAsync<PatientNotFoundException>(async () => 
            await GetServiceManager().PatientService.GetByIdAsync(Guid.NewGuid()));
    }
    
    [Test]
    public async Task UpdateAsync_ExistingPatientWithValidData_UpdatesPatient()
    {
        // Arrange
        var context = GetDbContext();
        var seededPatientDto = await PatientTestData.SeedPatient(context, GetIdentityUserManager());
        seededPatientDto.FirstName = "UpdatedFirstName";
        seededPatientDto.Email = "updatedEmail@example.com";
        
        // Act
        await GetServiceManager().PatientService.UpdateAsync(seededPatientDto);
        
        // Assert
        var result = context.Patients.Single(p => p.Id == seededPatientDto.Id);
        Assert.Multiple(() =>
        {
            Assert.That(result!.FirstName, Is.EqualTo(seededPatientDto.FirstName));
            Assert.That(result.Email, Is.EqualTo(seededPatientDto.Email));
        });
    }
    
    [Test]
    public async Task UpdateAsync_ExistingPatientWithInvalidData_Throws()
    {
        // Arrange
        var seededPatientDto = await PatientTestData.SeedPatient(GetDbContext(), GetIdentityUserManager());
        seededPatientDto.FirstName = "";
        seededPatientDto.Email = "";
        
        // Act & Assert
        Assert.ThrowsAsync<PatientBadRequestException>(() => GetServiceManager().PatientService.UpdateAsync(seededPatientDto));
    }
    
    [Test]
    public void UpdateAsync_NonExistingPatient_Throws()
    {
        // Arrange
        var patientDto = PatientTestData.CreateDto().Adapt<PatientDto>();
        patientDto.Id = Guid.NewGuid();
        
        // Act & Assert
        Assert.ThrowsAsync<PatientNotFoundException>(() => GetServiceManager().PatientService.UpdateAsync(patientDto));
    }
    
    [Test]
    public async Task DeleteAsync_ExistingPatient_DeletesPatientAndAccount()
    {
        // Arrange
        var context = GetDbContext();
        var seededPatientDto = await PatientTestData.SeedPatient(context, GetIdentityUserManager());
        
        // Act
        await GetServiceManager().PatientService.DeleteAsync(seededPatientDto.Id);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(context.Patients, Is.Empty);
            Assert.That(context.Accounts, Is.Empty);
            Assert.That(context.Users, Is.Empty);
            Assert.That(context.UserRoles, Is.Empty);
        });
    }
    
    [Test]
    public void DeleteAsync_NonExistingPatient_Throws()
    {
        // Act & Assert
        Assert.ThrowsAsync<PatientNotFoundException>(() => 
            GetServiceManager().PatientService.DeleteAsync(Guid.NewGuid()));
    }
}