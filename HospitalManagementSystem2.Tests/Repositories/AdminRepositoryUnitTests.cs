using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Models.Entities;
using HospitalManagementSystem2.Repositories;
using HospitalManagementSystem2.Tests.Helpers;
using HospitalManagementSystem2.Tests.TestData;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem2.Tests.Repositories;

public class AdminRepositoryUnitTests : IDisposable, IAsyncDisposable
{
    private readonly AdminRepository _sut;
    private readonly ApplicationDbContext _context;

    public AdminRepositoryUnitTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _sut = new AdminRepository(_context);
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task AddAsync_NewAdmin_AddsAdmin()
    {
        // Arrange
        var admin = AdminTestHelper.CreateAdmin();

        // Act
        await _sut.AddAsync(admin);
        await _context.SaveChangesAsync();

        // Assert 
        AdminTestHelper.AssertHasData(_context, admin);
    }

    [Fact]
    public async Task UpdateAsync_ExistingAdmin_UpdatesAdmin()
    {
        // Arrange
        var admin = AdminTestHelper.CreateAndSeedAdmin(_context);
        admin.FirstName = "UpdatedFirstName";
        admin.LastName = "UpdatedLastName";
        
        // Act
        await _sut.UpdateAsync(admin);
        await _context.SaveChangesAsync();
        
        // Assert
        AdminTestHelper.AssertHasData(_context, admin);
    }

    [Fact]
    public async Task UpdateAsync_ExistingAdminInvalidData_UpdatesAdmin()
    {
        // Arrange
        var admin = AdminTestHelper.CreateAndSeedAdmin(_context);
        admin.FirstName = string.Empty;
        admin.LastName = string.Empty;
        
        // Act & Assert
        await _sut.UpdateAsync(admin);
        AdminTestHelper.AssertHasData(_context, admin);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingAdmin_Throws()
    {
        // Arrange
        var admin = AdminTestHelper.CreateAdmin();
        admin.Id = Guid.NewGuid();

        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => _sut.UpdateAsync(admin));
        Assert.Contains(ErrorMessageData.SequenceNoElements, result.Message);
    }

    [Fact]
    public async Task RemoveAsync_ExistingAdmin_RemovesAdmin()
    {
        // Arrange
        var admin = AdminTestHelper.CreateAndSeedAdmin(_context);

        // Act
        await _sut.RemoveAsync(admin);
        await _context.SaveChangesAsync();

        // Assert
        AdminTestHelper.AssertHasNoData(_context, admin);
    }

    [Fact]
    public async Task RemoveAsync_NonExistingAdmin_Throws()
    {
        // Arrange
        var admin = AdminTestHelper.CreateAdmin();
        admin.Id = Guid.NewGuid();

        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => _sut.RemoveAsync(admin));
        Assert.Contains(ErrorMessageData.SequenceNoElements, result.Message);
    }
}