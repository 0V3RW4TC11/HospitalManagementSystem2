using HospitalManagementSystem2.Models.Entities;
using HospitalManagementSystem2.Utility;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace HospitalManagementSystem2.Tests.Utility
{
    public class AccountHelperTests
    {
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
        private readonly AccountHelper _accountHelper;

        public AccountHelperTests()
        {
            _userManagerMock = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
            _accountHelper = new AccountHelper(_userManagerMock.Object);
        }

        [Fact]
        public async Task CreateAsync_SuccessfulCreation_SetsUserAndUserId()
        {
            // Arrange
            //var account = new Account();
            var account = new Mock<Account> { CallBase = true }.Object;
            var username = "testuser";
            var password = "Test@123";
            var role = "Admin";

            _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<IdentityUser>(), password))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(m => m.AddToRoleAsync(It.IsAny<IdentityUser>(), role))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await _accountHelper.CreateAsync(account, role, username, password);

            // Assert
            Assert.NotNull(account.User);
            Assert.Equal(username, account.User.UserName);
            Assert.Equal(account.User.Id, account.UserId);
            _userManagerMock.Verify(m => m.CreateAsync(It.Is<IdentityUser>(u => u.UserName == username), password), Times.Once());
            _userManagerMock.Verify(m => m.AddToRoleAsync(It.Is<IdentityUser>(u => u.UserName == username), role), Times.Once());
        }

        [Fact]
        public async Task CreateAsync_CreateFails_ThrowsException()
        {
            // Arrange
            var account = new Mock<Account> { CallBase = true }.Object;
            var username = "testuser";
            var password = "Test@123";
            var role = "Admin";
            var errors = new[] { new IdentityError { Description = "User creation failed" } };

            _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<IdentityUser>(), password))
                .ReturnsAsync(IdentityResult.Failed(errors));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _accountHelper.CreateAsync(account, role, username, password));
            Assert.Equal("User creation failed", exception.Message);
            _userManagerMock.Verify(m => m.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Never());
        }

        [Fact]
        public async Task CreateAsync_AddToRoleFails_ThrowsException()
        {
            // Arrange
            var account = new Mock<Account> { CallBase = true }.Object;
            var username = "testuser";
            var password = "Test@123";
            var role = "Admin";
            var errors = new[] { new IdentityError { Description = "Role assignment failed" } };

            _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<IdentityUser>(), password))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(m => m.AddToRoleAsync(It.IsAny<IdentityUser>(), role))
                .ReturnsAsync(IdentityResult.Failed(errors));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _accountHelper.CreateAsync(account, role, username, password));
            Assert.Equal("Role assignment failed", exception.Message);
        }

        [Fact]
        public async Task UpdateAsync_ValidUser_CallsUpdateAsync()
        {
            // Arrange
            var account = new Mock<Account> { CallBase = true}.Object;
            account.User = new IdentityUser { UserName = "testuser" };

            _userManagerMock.Setup(m => m.UpdateAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await _accountHelper.UpdateAsync(account);

            // Assert
            _userManagerMock.Verify(m => m.UpdateAsync(It.Is<IdentityUser>(u => u.UserName == "testuser")), Times.Once());
        }

        [Fact]
        public async Task UpdateAsync_NullUser_ThrowsException()
        {
            // Arrange
            var account = new Mock<Account> { CallBase = true }.Object;
            account.Id = Guid.NewGuid();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _accountHelper.UpdateAsync(account));
            Assert.Equal(NotificationHelper.MissingData(nameof(Account.User), nameof(Account), account.Id.ToString()), exception.Message);
            _userManagerMock.Verify(m => m.UpdateAsync(It.IsAny<IdentityUser>()), Times.Never());
        }

        [Fact]
        public async Task DeleteAsync_SuccessfulDeletion_ClearsUserId()
        {
            // Arrange
            var account = new Mock<Account> { CallBase = true }.Object;
            account.User = new IdentityUser { UserName = "testuser" };
            account.UserId = Guid.NewGuid().ToString();

            _userManagerMock.Setup(m => m.DeleteAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await _accountHelper.DeleteAsync(account);

            // Assert
            Assert.Equal(string.Empty, account.UserId);
            _userManagerMock.Verify(m => m.DeleteAsync(It.Is<IdentityUser>(u => u.UserName == "testuser")), Times.Once());
        }

        [Fact]
        public async Task DeleteAsync_DeleteFails_ThrowsException()
        {
            // Arrange
            var account = new Mock<Account> { CallBase = true }.Object;
            account.User = new IdentityUser { UserName = "testuser" };
            var errors = new[] { new IdentityError { Description = "User deletion failed" } };

            _userManagerMock.Setup(m => m.DeleteAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(IdentityResult.Failed(errors));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _accountHelper.DeleteAsync(account));
            Assert.Equal("User deletion failed", exception.Message);
        }

        [Fact]
        public void CheckIdentityResult_Success_DoesNotThrow()
        {
            // Arrange
            var result = IdentityResult.Success;

            // Act & Assert
            AccountHelper.CheckIdentityResult(result); // Should not throw
        }

        [Fact]
        public void CheckIdentityResult_Failure_ThrowsException()
        {
            // Arrange
            var errors = new[]
            {
                new IdentityError { Description = "Error1" },
                new IdentityError { Description = "Error2" }
            };
            var result = IdentityResult.Failed(errors);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => AccountHelper.CheckIdentityResult(result));
            Assert.Equal("Error1, Error2", exception.Message);
        }
    }
}
