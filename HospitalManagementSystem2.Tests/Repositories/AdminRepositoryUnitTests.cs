using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Models.Entities;
using HospitalManagementSystem2.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem2.Tests.Repositories;

public class AdminRepositoryUnitTests : IDisposable, IAsyncDisposable
{
    private const string TestTitle = "ExampleTitle";
    private const string TestFirstName = "ExampleFirstName";
    private const string TestLastName = "ExampleLastName";
    private const string TestGender = "ExampleGender";
    private const string TestAddress = "ExampleAddress";
    private const string TestPhone = "ExamplePhone";
    private const string TestEmail = "ExampleEmail";
    private static readonly DateOnly TestDateOfBirth = DateOnly.FromDateTime(DateTime.UnixEpoch);
    private readonly AdminRepository _adminRepository;
    private readonly ApplicationDbContext _dbContext;

    public AdminRepositoryUnitTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _adminRepository = new AdminRepository(_dbContext);
    }

    public async ValueTask DisposeAsync()
    {
        await _dbContext.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task AddAsync_AddAndSaveDbContext_ShouldAdd()
    {
        // Arrange
        var admin = new Admin
        {
            Title = TestTitle,
            FirstName = TestFirstName,
            LastName = TestLastName,
            Gender = TestGender,
            Address = TestAddress,
            Phone = TestPhone,
            Email = TestEmail,
            DateOfBirth = TestDateOfBirth
        };

        // Act
        await _adminRepository.AddAsync(admin);
        await _dbContext.SaveChangesAsync();

        // Assert 
        var addedAdmin = await _dbContext.Admins.FirstOrDefaultAsync(a => a == admin);
        Assert.NotNull(addedAdmin);
    }

    [Fact]
    public async Task AddAsync_AddAndDontSaveDbContext_ShouldNotAdd()
    {
        // Arrange
        var admin = new Admin
        {
            Title = TestTitle,
            FirstName = TestFirstName,
            LastName = TestLastName,
            Gender = TestGender,
            Address = TestAddress,
            Phone = TestPhone,
            Email = TestEmail,
            DateOfBirth = TestDateOfBirth
        };

        // Act
        await _adminRepository.AddAsync(admin);

        // Assert 
        var addedAdmin = await _dbContext.Admins.FirstOrDefaultAsync(a => a == admin);
        Assert.Null(addedAdmin);
    }

    [Fact]
    public async Task UpdateAsync_ExistingAdminAndSaveDbContext_ShouldSave()
    {
        // Arrange
        Guid adminId;
        const string expectedFirstName = "UpdatedFirstName";

        {
            var admin = new Admin
            {
                Title = TestTitle,
                FirstName = TestFirstName,
                LastName = TestLastName,
                Gender = TestGender,
                Address = TestAddress,
                Phone = TestPhone,
                Email = TestEmail,
                DateOfBirth = TestDateOfBirth
            };
            await _dbContext.Admins.AddAsync(admin);
            await _dbContext.SaveChangesAsync();
            adminId = admin.Id;
        }

        // Act
        var existingAdmin = await _dbContext.Admins.AsNoTracking().FirstAsync(a => a.Id == adminId);
        existingAdmin.FirstName = expectedFirstName;
        await _adminRepository.UpdateAsync(existingAdmin);
        await _dbContext.SaveChangesAsync();

        // Assert
        Assert.True(await _dbContext.Admins.AnyAsync(a => a.Id == adminId && a.FirstName == expectedFirstName));
    }

    [Fact]
    public async Task UpdateAsync_ExistingAdminAndDontSaveDbContext_ShouldNotSave()
    {
        // Arrange
        Guid adminId;
        const string expectedFirstName = "UpdatedFirstName";

        {
            var admin = new Admin
            {
                Title = TestTitle,
                FirstName = TestFirstName,
                LastName = TestLastName,
                Gender = TestGender,
                Address = TestAddress,
                Phone = TestPhone,
                Email = TestEmail,
                DateOfBirth = TestDateOfBirth
            };
            await _dbContext.Admins.AddAsync(admin);
            await _dbContext.SaveChangesAsync();
            adminId = admin.Id;
        }

        // Act
        var existingAdmin = await _dbContext.Admins.AsNoTracking().FirstAsync(a => a.Id == adminId);
        existingAdmin.FirstName = expectedFirstName;
        await _adminRepository.UpdateAsync(existingAdmin);

        // Assert
        Assert.False(await _dbContext.Admins.AnyAsync(a => a.Id == adminId && a.FirstName == expectedFirstName));
    }

    [Fact]
    public async Task UpdateAsync_NonExistingAdmin_ShouldThrow()
    {
        // Arrange
        var admin = new Admin
        {
            Id = Guid.NewGuid(),
            Title = TestTitle,
            FirstName = TestFirstName,
            LastName = TestLastName,
            Gender = TestGender,
            Address = TestAddress,
            Phone = TestPhone,
            Email = TestEmail,
            DateOfBirth = TestDateOfBirth
        };

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() => _adminRepository.UpdateAsync(admin));
    }

    [Fact]
    public async Task RemoveAsync_ExistingAdminAndSaveDbContext_ShouldRemove()
    {
        // Arrange
        var admin = new Admin
        {
            Title = TestTitle,
            FirstName = TestFirstName,
            LastName = TestLastName,
            Gender = TestGender,
            Address = TestAddress,
            Phone = TestPhone,
            Email = TestEmail,
            DateOfBirth = TestDateOfBirth
        };
        await _dbContext.Admins.AddAsync(admin);
        await _dbContext.SaveChangesAsync();

        // Act
        await _adminRepository.RemoveAsync(admin);
        await _dbContext.SaveChangesAsync();

        // Assert
        Assert.False(await _dbContext.Admins.AnyAsync(a => a.Id == admin.Id));
    }

    [Fact]
    public async Task RemoveAsync_ExistingAdminAndDontSaveDbContext_ShouldNotRemove()
    {
        // Arrange
        var admin = new Admin
        {
            Title = TestTitle,
            FirstName = TestFirstName,
            LastName = TestLastName,
            Gender = TestGender,
            Address = TestAddress,
            Phone = TestPhone,
            Email = TestEmail,
            DateOfBirth = TestDateOfBirth
        };
        await _dbContext.Admins.AddAsync(admin);
        await _dbContext.SaveChangesAsync();

        // Act
        await _adminRepository.RemoveAsync(admin);

        // Assert
        Assert.True(await _dbContext.Admins.AnyAsync(a => a.Id == admin.Id));
    }

    [Fact]
    public async Task RemoveAsync_NonExistingAdmin_ShouldThrow()
    {
        // Arrange
        var admin = new Admin
        {
            Title = TestTitle,
            FirstName = TestFirstName,
            LastName = TestLastName,
            Gender = TestGender,
            Address = TestAddress,
            Phone = TestPhone,
            Email = TestEmail,
            DateOfBirth = TestDateOfBirth
        };

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() => _adminRepository.RemoveAsync(admin));
    }
}