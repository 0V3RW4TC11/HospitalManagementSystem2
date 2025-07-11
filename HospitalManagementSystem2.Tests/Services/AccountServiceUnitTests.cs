using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Models.Entities;
using HospitalManagementSystem2.Repositories;
using HospitalManagementSystem2.Services;
using Microsoft.AspNetCore.Identity;
using MockQueryable;
using Moq;

namespace HospitalManagementSystem2.Tests.Services;

public class AccountServiceUnitTests
{
    private readonly Mock<IDbContext> _mockContext;
    private readonly Mock<IAccountRepository> _mockRepo;
    private readonly Mock<UserManager<IdentityUser>> _mockUserManager;
    private readonly AccountService _sut;

    public AccountServiceUnitTests()
    {
        _mockContext = new Mock<IDbContext>();
        _mockRepo = new Mock<IAccountRepository>();
        _mockUserManager = new Mock<UserManager<IdentityUser>>(
            Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
        _sut = new AccountService(_mockContext.Object, _mockRepo.Object, _mockUserManager.Object);
    }

    [Fact]
    public async Task CreateAsync_SuccessWithAllDependencies_AddsAccount()
    {
        // Arrange
        var expectedUserId = Guid.NewGuid();
        var expectedIdentityUserId = Guid.NewGuid().ToString();
        var expectedRole = "testRole";
        var expectedUsername = "testUsername";
        var expectedPassword = "testPassword";
        Account? account = null;
        string? actualIdentityUserIdForRole = null;
        string? actualRole = null;
        string? actualPassword = null;

        _mockUserManager.Setup(u
                => u.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .Callback((IdentityUser user, string password) =>
            {
                user.Id = expectedIdentityUserId;
                actualPassword = password;
            })
            .ReturnsAsync(IdentityResult.Success);
        _mockUserManager.Setup(u
                => u.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .Callback((IdentityUser user, string role) =>
            {
                actualIdentityUserIdForRole = user.Id;
                actualRole = role;
            })
            .ReturnsAsync(IdentityResult.Success);
        _mockRepo.Setup(r
                => r.AddAsync(It.IsAny<Account>()))
            .Callback((Account acc) => { account = acc; })
            .Returns(Task.CompletedTask);
        _mockContext.Setup(c => c.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        await _sut.CreateAsync(expectedUserId, expectedRole, expectedUsername, expectedPassword);

        // Assert
        Assert.Equal(expectedPassword, actualPassword);
        Assert.Equal(expectedRole, actualRole);
        Assert.Equal(expectedIdentityUserId, actualIdentityUserIdForRole);
        Assert.Equal(expectedUserId, account?.UserId);
        Assert.Equal(expectedIdentityUserId, account?.IdentityUserId);
        _mockContext.Verify(c => c.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_UserManagerCreateAsyncFails_ShouldThrowExceptionWithMessage()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var role = "testRole";
        var username = "testUsername";
        var password = "testPassword";
        var expected = "testError";

        _mockUserManager.Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .Callback((IdentityUser u, string p) => { u.Id = Guid.Empty.ToString(); })
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = expected }));
        _mockUserManager.Setup(u => u.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Account>())).Returns(Task.CompletedTask);
        _mockContext.Setup(c => c.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => _sut.CreateAsync(userId, role, username, password));
        Assert.Equal(expected, result.Message);
    }

    [Fact]
    public async Task CreateAsync_UserManagerAddToRoleAsyncFails_ShouldThrowExceptionWithMessage()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var role = "testRole";
        var username = "testUsername";
        var password = "testPassword";
        var expected = "testError";

        _mockUserManager.Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .Callback((IdentityUser user, string password) => { user.Id = Guid.NewGuid().ToString(); })
            .ReturnsAsync(IdentityResult.Success);
        _mockUserManager.Setup(u => u.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = expected }));
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Account>())).Returns(Task.CompletedTask);
        _mockContext.Setup(c => c.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => _sut.CreateAsync(userId, role, username, password));
        Assert.Equal(expected, result.Message);
    }

    [Fact]
    public async Task GetUserIdByIdentityIdAsync_ExistingIdentityId_ReturnsExistingUserId()
    {
        // Arrange
        var account = new Account { UserId = Guid.NewGuid(), IdentityUserId = Guid.NewGuid().ToString() };
        var mockData
            = new List<Account> { account }.BuildMock();
        _mockRepo.Setup(r => r.Accounts).Returns(mockData);

        // Act
        var returnedUserId = await _sut.GetUserIdByIdentityIdAsync(account.IdentityUserId);

        // Assert
        Assert.Equal(account.UserId, returnedUserId);
    }

    [Fact]
    public async Task GetUserIdByIdentityIdAsync_NonExistingIdentityId_ReturnsEmptyGuid()
    {
        // Arrange
        var mockData = new List<Account>().BuildMock();
        _mockRepo.Setup(r => r.Accounts).Returns(mockData);

        // Act
        var returnedUserId = await _sut.GetUserIdByIdentityIdAsync(Guid.NewGuid().ToString());

        // Assert
        Assert.Equal(Guid.Empty, returnedUserId);
    }

    [Fact]
    public async Task DeleteByUserIdAsync_ExistingUserId_ShouldRemoveAccountAndIdentityUser()
    {
        // Arrange
        var account = new Account { UserId = Guid.NewGuid(), IdentityUserId = Guid.NewGuid().ToString() };
        var mockAccountData = new List<Account> { account };
        var mockIdentityData = new List<IdentityUser> { new() { Id = account.IdentityUserId } };

        _mockRepo.Setup(r => r.Accounts).Returns(mockAccountData.BuildMock());
        _mockRepo.Setup(r => r.Remove(It.IsAny<Account>()))
            .Callback((Account a) =>
            {
                var returnedAccount = mockAccountData.Find(x => x == a);
                if (returnedAccount != null) mockAccountData.Remove(returnedAccount);
            });
        _mockContext.Setup(c => c.SaveChangesAsync()).Returns(Task.CompletedTask);
        _mockUserManager.Setup(u => u.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((string id) => mockIdentityData.FirstOrDefault(a => a.Id == id));
        _mockUserManager.Setup(u => u.DeleteAsync(It.IsAny<IdentityUser>()))
            .ReturnsAsync((IdentityUser user) =>
            {
                var success = mockIdentityData.Remove(user);
                return success
                    ? IdentityResult.Success
                    : IdentityResult.Failed(new IdentityError { Description = "Failed to delete IdentityUser" });
            });

        // Act
        await _sut.DeleteByUserIdAsync(account.UserId);

        // Assert
        _mockContext.Verify(c => c.SaveChangesAsync(), Times.Once);
        Assert.DoesNotContain(mockAccountData, x => x == account);
        Assert.DoesNotContain(mockIdentityData, x => x.Id == account.IdentityUserId);
    }

    [Fact]
    public async Task DeleteByUserIdAsync_NonExistingUserId_ShouldThrowWithMessage()
    {
        // Arrange
        var account = new Account { UserId = Guid.NewGuid(), IdentityUserId = Guid.NewGuid().ToString() };
        var mockAccountData = new List<Account>();
        var expectedMessage = $"No account for user Id {account.UserId.ToString()}";

        _mockRepo.Setup(r => r.Accounts).Returns(mockAccountData.BuildMock());

        // Act & Assert
        var result = await Assert.ThrowsAnyAsync<Exception>(() => _sut.DeleteByUserIdAsync(account.UserId));
        Assert.Equal(expectedMessage, result.Message);
    }
}