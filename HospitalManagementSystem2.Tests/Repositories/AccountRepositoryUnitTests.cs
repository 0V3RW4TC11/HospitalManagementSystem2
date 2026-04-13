using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Models.Entities;
using HospitalManagementSystem2.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem2.Tests.Repositories;

public class AccountRepositoryUnitTests : IDisposable, IAsyncDisposable
{
    private readonly ApplicationDbContext _dbContext;
    private readonly AccountRepository _sut;

    public AccountRepositoryUnitTests()
    {
        // Arrange the in-memory database context
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _sut = new AccountRepository(_dbContext);
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
    public void Accounts_ReturnsQueryable()
    {
        // Act
        var queryable = _sut.Accounts;

        // Assert
        Assert.IsAssignableFrom<IQueryable<Account>>(queryable);
    }

    [Fact]
    public async Task AddAsync_AddAndSaveDbContext_ShouldAddAccount()
    {
        // Arrange
        var account = new Account { UserId = Guid.NewGuid(), IdentityUserId = Guid.NewGuid().ToString() };

        // Act
        await _sut.AddAsync(account);
        await _dbContext.SaveChangesAsync();

        // Assert
        var result = await _dbContext.Accounts.FirstOrDefaultAsync(a => a == account);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task AddAsync_AddAndDontSaveDbContext_ShouldNotAddAccount()
    {
        // Arrange
        var account = new Account { UserId = Guid.NewGuid(), IdentityUserId = Guid.NewGuid().ToString() };

        // Act
        await _sut.AddAsync(account);

        // Assert
        var result = await _dbContext.Accounts.FirstOrDefaultAsync(a => a == account);
        Assert.Null(result);
    }

    [Fact]
    public async Task Remove_ExistingAccountAndDbSaved_ShouldRemoveAccount()
    {
        // Arrange
        var account = new Account { UserId = Guid.NewGuid(), IdentityUserId = Guid.NewGuid().ToString() };
        await _dbContext.Accounts.AddAsync(account);
        await _dbContext.SaveChangesAsync();

        // Act
        await _sut.RemoveAsync(account);
        await _dbContext.SaveChangesAsync();

        // Assert
        var result = await _dbContext.Accounts.FirstOrDefaultAsync(a => a == account);
        Assert.Null(result);
    }

    [Fact]
    public async Task Remove_ExistingAccountAndDbNotSaved_ShouldNotRemoveAccount()
    {
        // Arrange
        var account = new Account { UserId = Guid.NewGuid(), IdentityUserId = Guid.NewGuid().ToString() };
        await _dbContext.Accounts.AddAsync(account);
        await _dbContext.SaveChangesAsync();

        // Act
        await _sut.RemoveAsync(account);

        // Assert
        var result = await _dbContext.Accounts.FirstOrDefaultAsync(a => a == account);
        Assert.NotNull(result);
    }

    [Fact]
    public void Remove_NonExistingAccountAndDbSaved_ShouldThrow()
    {
        // Arrange
        var account = new Account { UserId = Guid.NewGuid(), IdentityUserId = Guid.NewGuid().ToString() };

        // Act & Assert
        Assert.ThrowsAnyAsync<Exception>(async () =>
        {
            await _sut.RemoveAsync(account);
            _dbContext.SaveChanges();
        });
    }
}