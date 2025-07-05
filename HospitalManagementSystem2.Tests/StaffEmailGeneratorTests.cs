using HospitalManagementSystem2.Models.Entities;
using HospitalManagementSystem2.Services;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace HospitalManagementSystem2.Tests.Services
{
    public class StaffEmailGeneratorTests
    {
        private readonly Mock<UserManager<IdentityUser>> _mockUserManager;
        private readonly StaffEmailGenerator _emailGenerator;

        public StaffEmailGeneratorTests()
        {
            _mockUserManager = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
            _emailGenerator = new StaffEmailGenerator(_mockUserManager.Object);
        }

        [Fact]
        public async Task GenerateEmailAsync_ValidInputNoConflict_ReturnsExpectedEmail()
        {
            // Arrange
            var staff = new Mock<Staff> { CallBase = true }.Object;
            staff.FirstName = "John";
            staff.LastName = "Doe";
            string domain = "hospital.com";
            _mockUserManager.Setup(m => m.FindByEmailAsync("john.doe@hospital.com"))
                .ReturnsAsync((IdentityUser)null);

            // Act
            var result = await _emailGenerator.GenerateEmailAsync(staff, domain);

            // Assert
            Assert.Equal("john.doe@hospital.com", result);
        }

        [Fact]
        public async Task GenerateEmailAsync_EmailConflict_ReturnsNumberedEmail()
        {
            // Arrange
            var staff = new Mock<Staff> { CallBase = true }.Object;
            staff.FirstName = "John";
            staff.LastName = "Doe";
            string domain = "hospital.com";
            _mockUserManager.SetupSequence(m => m.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new IdentityUser()) // john.doe@hospital.com exists
                .ReturnsAsync(new IdentityUser()) // john.doe1@hospital.com exists
                .ReturnsAsync((IdentityUser)null); // john.doe2@hospital.com available

            // Act
            var result = await _emailGenerator.GenerateEmailAsync(staff, domain);

            // Assert
            Assert.Equal("john.doe2@hospital.com", result);
        }

        [Fact]
        public async Task GenerateEmailAsync_MaxTriesExceeded_ThrowsException()
        {
            // Arrange
            var staff = new Mock<Staff> { CallBase = true }.Object;
            staff.FirstName = "John";
            staff.LastName = "Doe";
            string domain = "hospital.com";
            _mockUserManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new IdentityUser()); // All emails exist

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _emailGenerator.GenerateEmailAsync(staff, domain));
        }

        [Fact]
        public async Task GenerateEmailAsync_EmptyFirstName_ThrowsException()
        {
            // Arrange
            var staff = new Mock<Staff> { CallBase = true }.Object;
            staff.FirstName = "";
            staff.LastName = "Doe";
            string domain = "hospital.com";

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _emailGenerator.GenerateEmailAsync(staff, domain));
        }

        [Fact]
        public async Task GenerateEmailAsync_NullFirstName_ThrowsException()
        {
            // Arrange
            var staff = new Mock<Staff> { CallBase = true }.Object;
            staff.FirstName = "";
            staff.LastName = "Doe";
            string domain = "hospital.com";

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _emailGenerator.GenerateEmailAsync(staff, domain));
        }

        [Fact]
        public async Task GenerateEmailAsync_OnlyFirstName_ReturnsFirstNameEmail()
        {
            // Arrange
            var staff = new Mock<Staff> { CallBase = true }.Object;
            staff.FirstName = "John";
            staff.LastName = "";
            string domain = "hospital.com";
            _mockUserManager.Setup(m => m.FindByEmailAsync("john@hospital.com"))
                .ReturnsAsync((IdentityUser)null);

            // Act
            var result = await _emailGenerator.GenerateEmailAsync(staff, domain);

            // Assert
            Assert.Equal("john@hospital.com", result);
        }
    }
}
