using DataTransfer.Admin;
using Domain.Constants;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Configurations;
using Persistence.Repositories;
using Services;
using Services.Abstractions;
using TestData;

namespace Persistence.Tests;

internal class AdminServiceTests : PersistenceTestBase
{
    public AdminServiceTests() : base(services =>
    {
        services.AddScoped<IRepositoryManager, AdminTestRepoManager>();
        services.AddScoped<IServiceManager, AdminTestServiceManager>();
    })
    {
    }
    
    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        SeedAdminRole();
    }

    private void SeedAdminRole()
    {
        var roleManager = GetServiceProvider().GetRequiredService<RoleManager<IdentityRole>>();
        roleManager.CreateAsync(new IdentityRole(AuthRoles.Admin)).GetAwaiter().GetResult();
    }

    private async Task<AdminDto> SeedAdmin()
    {
        var adminDto = AdminTestData.CreateDto();
        var context = GetDbContext();
        var admin = adminDto.Adapt<Admin>();
        
        context.Admins.Add(admin);
        await context.SaveChangesAsync();
        
        var identity = new IdentityUser {UserName = AdminTestData.ExpectedUsername};
        await GetServiceProvider().GetRequiredService<UserManager<IdentityUser>>()
            .CreateAsync(identity, adminDto.Password);
        
        context.Accounts.Add(new Account {UserId = admin.Id, IdentityUserId = identity.Id});
        await context.SaveChangesAsync();
        
        return admin.Adapt<AdminDto>();
    }

    [Test]
    public async Task CreateAdmin_AdminWithValidData_CreatesAdminAndAccount()
    {
        // Arrange
        var dto = AdminTestData.CreateDto();
        
        // Act
        await GetServiceManager().AdminService.CreateAsync(dto);

        // Assert
        // Has unique Admin record
        var admin = GetDbContext().Admins.Single(a => 
            a.FirstName == dto.FirstName &&
            a.LastName == dto.LastName &&
            a.Gender == dto.Gender &&
            a.Address == dto.Address &&
            a.Phone == dto.Phone &&
            a.Email == dto.Email &&
            a.DateOfBirth == dto.DateOfBirth);

        // Has unique Account record
        var account = GetDbContext().Accounts.Single(a => a.UserId == admin!.Id);

        // Has unique Identity record
        var identityUser = GetDbContext().Users.Single(a => a.Id == account!.IdentityUserId);

        // Has expected username
        Assert.That(identityUser!.UserName, Is.EqualTo(AdminTestData.ExpectedUsername));
        
        // Has expected role
        var adminRoleId = GetDbContext().Roles.Single(r => r.Name == AuthRoles.Admin).Id;
        Assert.That(GetDbContext().UserRoles.Any(r => r.UserId == identityUser.Id && r.RoleId == adminRoleId));
    }

    [Test]
    public void CreateAsync_AdminWithInvalidData_Throws()
    {
        // Arrange
        var dto = AdminTestData.CreateDto();
        dto.FirstName = string.Empty;
        dto.Phone = string.Empty;
        dto.Email = string.Empty;
        
        // Act & Assert
        Assert.ThrowsAsync<AdminBadRequest>(async () => await GetServiceManager().AdminService.CreateAsync(dto));
    }
    
    [Test]
    public async Task CreateAsync_DuplicateAdmin_Throws()
    {
        // Arrange
        await SeedAdmin();
        var dto = AdminTestData.CreateDto();
        
        // Act & Assert
        Assert.ThrowsAsync<AdminBadRequest>(async () => await GetServiceManager().AdminService.CreateAsync(dto));
    }
    
    [Test]
    public async Task GetByIdAsync_ExistingAdmin_ReturnsAdmin()
    {
        // Arrange
        var seededAdminDto = await SeedAdmin();
        
        // Act
        var dto = await GetServiceManager().AdminService.GetByIdAsync(seededAdminDto.Id);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(dto.FirstName, Is.EqualTo(seededAdminDto.FirstName));
            Assert.That(dto.LastName, Is.EqualTo(seededAdminDto.LastName));
            Assert.That(dto.Gender, Is.EqualTo(seededAdminDto.Gender));
            Assert.That(dto.Address, Is.EqualTo(seededAdminDto.Address));
            Assert.That(dto.Phone, Is.EqualTo(seededAdminDto.Phone));
            Assert.That(dto.Email, Is.EqualTo(seededAdminDto.Email));
            Assert.That(dto.DateOfBirth, Is.EqualTo(seededAdminDto.DateOfBirth));
        });
    }
    
    [Test]
    public void GetByIdAsync_NonExistingAdmin_Throws()
    {
        // Act & Assert
        Assert.ThrowsAsync<AdminNotFoundException>(async () => 
            await GetServiceManager().AdminService.GetByIdAsync(Guid.NewGuid()));
    }
    
    [Test]
    public async Task UpdateAsync_ExistingAdminWithValidData_UpdatesAdmin()
    {
        // Arrange
        var seededAdminDto = await SeedAdmin();
        seededAdminDto.FirstName = "UpdatedFirstName";
        seededAdminDto.Email = "updatedEmail@example.com";
        
        // Act
        await GetServiceManager().AdminService.UpdateAsync(seededAdminDto);
        
        // Assert
        var result = GetDbContext().Admins.Single(a => a.Id == seededAdminDto.Id);
        Assert.Multiple(() =>
        {
            Assert.That(result!.FirstName, Is.EqualTo(seededAdminDto.FirstName));
            Assert.That(result.Email, Is.EqualTo(seededAdminDto.Email));
        });
    }
    
    [Test]
    public async Task UpdateAsync_ExistingAdminWithInvalidData_Throws()
    {
        // Arrange
        var seededAdminDto = await SeedAdmin();
        seededAdminDto.FirstName = "";
        seededAdminDto.Email = "";
        
        // Act & Assert
        Assert.ThrowsAsync<AdminBadRequest>(() => GetServiceManager().AdminService.UpdateAsync(seededAdminDto));
    }
    
    [Test]
    public void UpdateAsync_NonExistingAdmin_Throws()
    {
        // Arrange
        var dto = AdminTestData.CreateDto().Adapt<AdminDto>();
        dto.Id = Guid.NewGuid();
        
        // Act & Assert
        Assert.ThrowsAsync<AdminNotFoundException>(() => GetServiceManager().AdminService.UpdateAsync(dto));
    }
    
    [Test]
    public async Task DeleteAsync_ExistingAdmin_DeletesAdminAndAccount()
    {
        // Arrange
        var seededAdminDto = await SeedAdmin();
        
        // Act
        await GetServiceManager().AdminService.DeleteAsync(seededAdminDto.Id);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(GetDbContext().Admins, Is.Empty);
            Assert.That(GetDbContext().Accounts, Is.Empty);
            Assert.That(GetDbContext().Users, Is.Empty);
            Assert.That(GetDbContext().UserRoles, Is.Empty);
        });
    }
    
    [Test]
    public void DeleteAsync_NonExistingAdmin_Throws()
    {
        // Arrange
        var id = Guid.NewGuid();
        
        // Act & Assert
        Assert.ThrowsAsync<AdminNotFoundException>(() => GetServiceManager().AdminService.DeleteAsync(id));
    }
}

internal class AdminTestRepoManager : IRepositoryManager
{
    public AdminTestRepoManager(RepositoryDbContext context, UserManager<IdentityUser> userManager)
    {
        var lazyAccountRepository = new Lazy<IAccountRepository>(() => new AccountRepository(context));
        var lazyAdminRepository = new Lazy<IAdminRepository>(() => new AdminRepository(context));
        var lazyIdentityProvider = new Lazy<IIdentityProvider>(() => new IdentityProvider(userManager));
        var lazyUnitOfWork = new Lazy<IUnitOfWork>(() => new UnitOfWork(context));
        
        AccountRepository = lazyAccountRepository.Value;
        AdminRepository = lazyAdminRepository.Value;
        AttendanceRepository = null!;
        DoctorRepository = null!;
        DoctorSpecializationRepository = null!;
        PatientRepository = null!;
        SpecializationRepository = null!;
        IdentityProvider = lazyIdentityProvider.Value;
        UnitOfWork = lazyUnitOfWork.Value;
    }
    
    public IAccountRepository AccountRepository { get; }
    public IAdminRepository AdminRepository { get; }
    public IAttendanceRepository AttendanceRepository { get; }
    public IDoctorRepository DoctorRepository { get; }
    public IDoctorSpecializationRepository DoctorSpecializationRepository { get; }
    public IPatientRepository PatientRepository { get; }
    public ISpecializationRepository SpecializationRepository { get; }
    public IIdentityProvider IdentityProvider { get; }
    public IUnitOfWork UnitOfWork { get; }
}

internal class AdminTestServiceManager : IServiceManager
{
    public AdminTestServiceManager(IRepositoryManager repositoryManager)
    {
        var lazyAccountService = new Lazy<IAccountService>(() => new AccountService(repositoryManager));
    
        var lazyStaffEmailService = new Lazy<IStaffEmailService>(() => new StaffEmailService(repositoryManager.IdentityProvider));
    
        var lazyAdminService = new Lazy<IAdminService>(() => new AdminService(
            repositoryManager, 
            lazyAccountService.Value,
            lazyStaffEmailService.Value));
    
        AdminService = lazyAdminService.Value;
        PatientService = null!;
        DoctorService = null!;
        SpecializationService = null!;
        AttendanceService = null!;
    }

    public IAdminService AdminService { get; }
    public IPatientService PatientService { get; }
    public IDoctorService DoctorService { get; }
    public ISpecializationService SpecializationService { get; }
    public IAttendanceService AttendanceService { get; }
}