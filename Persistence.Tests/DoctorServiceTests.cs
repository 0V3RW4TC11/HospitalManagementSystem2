using DataTransfer.Doctor;
using DataTransfer.Specialization;
using Domain.Constants;
using Domain.Exceptions;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TestData;

namespace Persistence.Tests;

internal sealed class DoctorServiceTests : PersistenceTestBase
{
    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        SeedDoctorRole();
    }

    private void SeedDoctorRole()
    {
        var roleManager = GetServiceProvider().GetRequiredService<RoleManager<IdentityRole>>();
        roleManager.CreateAsync(new IdentityRole(AuthRoles.Doctor)).GetAwaiter().GetResult();
    }

    private async Task<HashSet<Guid>> SeedSpecsAndReturnGuidSet(params SpecializationCreateDto[] specializations)
    {
        var result = 
            await SpecializationTestData.SeedSpecializationsAsync(GetDbContext(), specializations);
        
        return result.Select(s => s.Id).ToHashSet();
    }

    [Test]
    public async Task CreateAsync_DoctorWithValidData_CreatesDoctorAndDocSpecsAndAccount()
    {
        // Arrange
        var context = GetDbContext();
        var spec1Dto = SpecializationTestData.CreateSpec1Dto();
        var spec2Dto = SpecializationTestData.CreateSpec2Dto();
        var specIds = await SeedSpecsAndReturnGuidSet(spec1Dto, spec2Dto);
        var doctorCreateDto = DoctorTestData.CreateDto(specIds);
        
        // Act
        await GetServiceManager().DoctorService.CreateAsync(doctorCreateDto);
        
        // Assert
        // Has unique Doctor record
        var doctor = context.Doctors.Single(d => 
            d.FirstName == doctorCreateDto.FirstName &&
            d.LastName == doctorCreateDto.LastName &&
            d.Gender == doctorCreateDto.Gender &&
            d.Address == doctorCreateDto.Address &&
            d.Phone == doctorCreateDto.Phone &&
            d.Email == doctorCreateDto.Email &&
            d.DateOfBirth == doctorCreateDto.DateOfBirth);
        
        // Has unique DocSpecs record
        Assert.That(context.DoctorSpecializations
            .Where(ds => ds.DoctorId == doctor.Id)
            .All(ds => specIds.Contains(ds.SpecializationId)));
        
        // Has unique Account record
        var account = context.Accounts.Single(a => a.UserId == doctor.Id);
        
        // Has unique Identity record
        var identityUser = context.Users.Single(a => a.Id == account.IdentityUserId);
        
        // Has expected username
        Assert.That(identityUser.UserName, Is.EqualTo(DoctorTestData.ExpectedUsername));
        
        // Has expected AuthRole
        var doctorRoleId = context.Roles.Single(r => r.Name == AuthRoles.Doctor).Id;
        Assert.That(context.UserRoles.Any(r => r.UserId == identityUser.Id && r.RoleId == doctorRoleId));
    }
    
    [Test]
    public async Task CreateAsync_DoctorWithInvalidData_Throws()
    {
        // Arrange
        var spec1Dto = SpecializationTestData.CreateSpec1Dto();
        var specIds = await SeedSpecsAndReturnGuidSet(spec1Dto);
        var doctorCreateDto = DoctorTestData.CreateDto(specIds);
        doctorCreateDto.FirstName = string.Empty;
        doctorCreateDto.Email = string.Empty;
        
        // Act & Assert
        Assert.ThrowsAsync<DoctorBadRequestException>(() => 
            GetServiceManager().DoctorService.CreateAsync(doctorCreateDto));
    }
    
    [Test]
    public async Task CreateAsync_DoctorWithDuplicateData_Throws()
    {
        // Arrange
        var context = GetDbContext();
        var spec1Dto = SpecializationTestData.CreateSpec1Dto();
        var specIds = await SeedSpecsAndReturnGuidSet(spec1Dto);
        await DoctorTestData.SeedDoctor(context, GetIdentityUserManager(), specIds);
        var doctorCreateDto = DoctorTestData.CreateDto(specIds);
        
        // Act & Assert
        Assert.ThrowsAsync<DoctorBadRequestException>(() => 
            GetServiceManager().DoctorService.CreateAsync(doctorCreateDto));
    }
    
    [Test]
    public void CreateAsync_DoctorWithNoSpecs_Throws()
    {
        // Arrange
        var doctorCreateDto = DoctorTestData.CreateDto(new HashSet<Guid>());
        
        // Act & Assert
        Assert.ThrowsAsync<DoctorBadRequestException>(() => GetServiceManager().DoctorService.CreateAsync(doctorCreateDto));
    }
    
    [Test]
    public async Task GetByIdAsync_ExistingDoctor_ReturnsDoctor()
    {
        // Arrange
        var context = GetDbContext();
        var spec1Dto = SpecializationTestData.CreateSpec1Dto();
        var specIds = await SeedSpecsAndReturnGuidSet(spec1Dto);
        var seededDoctorDto = await DoctorTestData.SeedDoctor(context, GetIdentityUserManager(), specIds);
        
        // Act
        var result = await GetServiceManager().DoctorService.GetByIdAsync(seededDoctorDto.Id);
        
        // Assert
        Assert.That(result, Is.Not.Null);
        
        Assert.Multiple(() =>
        {
            Assert.That(result.FirstName, Is.EqualTo(seededDoctorDto.FirstName));
            Assert.That(result.LastName, Is.EqualTo(seededDoctorDto.LastName));
            Assert.That(result.Gender, Is.EqualTo(seededDoctorDto.Gender));
            Assert.That(result.Address, Is.EqualTo(seededDoctorDto.Address));
            Assert.That(result.Phone, Is.EqualTo(seededDoctorDto.Phone));
            Assert.That(result.Email, Is.EqualTo(seededDoctorDto.Email));
            Assert.That(result.DateOfBirth, Is.EqualTo(seededDoctorDto.DateOfBirth));
        });
        
        Assert.That(specIds.ToList(), Is.EqualTo(result.SpecializationIds));
    }
    
    [Test]
    public void GetByIdAsync_NonExistingDoctor_Throws()
    {
        // Act & Assert
        Assert.ThrowsAsync<DoctorNotFoundException>(() => GetServiceManager().DoctorService.GetByIdAsync(Guid.NewGuid()));
    }
    
    [Test]
    public async Task UpdateAsync_ExistingDoctorWithValidData_UpdatesDoctor()
    {
        // Arrange
        var context = GetDbContext();
        var spec1Dto = SpecializationTestData.CreateSpec1Dto();
        var spec2Dto = SpecializationTestData.CreateSpec2Dto();
        var specIds = await SeedSpecsAndReturnGuidSet(spec1Dto, spec2Dto);
        var specIdsSubset = new HashSet<Guid> {specIds.First()};
        var seededDoctorDto = await DoctorTestData.SeedDoctor(context, GetIdentityUserManager(), specIdsSubset);
        
        seededDoctorDto.FirstName = "UpdatedFirstName";
        seededDoctorDto.SpecializationIds = specIds;

        // Act
        await GetServiceManager().DoctorService.UpdateAsync(seededDoctorDto);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(context.Doctors.Any(d => d.Id == seededDoctorDto.Id && d.FirstName == "UpdatedFirstName"));
            Assert.That(context.DoctorSpecializations
                .Where(ds => ds.DoctorId == seededDoctorDto.Id)
                .All(ds => specIds.Contains(ds.SpecializationId)));
        });
    }
    
    [Test]
    public async Task UpdateAsync_ExistingDoctorWithInvalidData_Throws()
    {
        // Arrange
        var context = GetDbContext();
        var spec1Dto = SpecializationTestData.CreateSpec1Dto();
        var specIds = await SeedSpecsAndReturnGuidSet(spec1Dto);
        var seededDoctorDto = await DoctorTestData.SeedDoctor(context, GetIdentityUserManager(), specIds);
        
        seededDoctorDto.FirstName = "";
        seededDoctorDto.LastName = "";

        // Act & Assert
        Assert.ThrowsAsync<DoctorBadRequestException>(() => GetServiceManager().DoctorService.UpdateAsync(seededDoctorDto));
    }
    
    [Test]
    public async Task UpdateAsync_ExistingDoctorWithNoSpecs_Throws()
    {
        // Arrange
        var context = GetDbContext();
        var seededDoctorDto = await DoctorTestData.SeedDoctor(context, GetIdentityUserManager(), 
            new HashSet<Guid>());

        // Act & Assert
        Assert.ThrowsAsync<DoctorBadRequestException>(() => GetServiceManager().DoctorService.UpdateAsync(seededDoctorDto));
    }
    
    [Test]
    public async Task UpdateAsync_NonExistingDoctor_Throws()
    {
        // Arrange
        var context = GetDbContext();
        var spec1Dto = SpecializationTestData.CreateSpec1Dto();
        var specIds = await SeedSpecsAndReturnGuidSet(spec1Dto);
        var doctorDto = DoctorTestData.CreateDto(specIds).Adapt<DoctorDto>();
        doctorDto.Id = Guid.NewGuid();

        // Act & Assert
        Assert.ThrowsAsync<DoctorNotFoundException>(() => GetServiceManager().DoctorService.UpdateAsync(doctorDto));
    }
    
    [Test]
    public async Task DeleteAsync_ExistingDoctor_DeletesDoctorAndDocSpecsAndAccount()
    {
        // Arrange
        var context = GetDbContext();
        var spec1Dto = SpecializationTestData.CreateSpec1Dto();
        var specIds = await SeedSpecsAndReturnGuidSet(spec1Dto);
        var seededDoctorDto = await DoctorTestData.SeedDoctor(context, GetIdentityUserManager(), specIds);
        
        // Act
        await GetServiceManager().DoctorService.DeleteAsync(seededDoctorDto.Id);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(context.Doctors, Is.Empty);
            Assert.That(context.DoctorSpecializations, Is.Empty);
            Assert.That(context.Accounts, Is.Empty);
            Assert.That(context.Users, Is.Empty);
            Assert.That(context.UserRoles, Is.Empty);
        });
    }
    
    [Test]
    public async Task DeleteAsync_NonExistingDoctor_Throws()
    {
        // Arrange
        var context = GetDbContext();
        var spec1Dto = SpecializationTestData.CreateSpec1Dto();
        var specIds = await SeedSpecsAndReturnGuidSet(spec1Dto);
        var doctorDto = DoctorTestData.CreateDto(specIds).Adapt<DoctorDto>();
        doctorDto.Id = Guid.NewGuid();
        
        // Act & Assert
        Assert.ThrowsAsync<DoctorNotFoundException>(() => GetServiceManager().DoctorService.DeleteAsync(doctorDto.Id));
    }
}