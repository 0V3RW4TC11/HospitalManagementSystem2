using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Models.Entities;
using HospitalManagementSystem2.Services;
using HospitalManagementSystem2.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using System.Linq.Expressions;

namespace HospitalManagementSystem2.Tests.Services
{
    public class AdminRepositoryTests
    {
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly Mock<DbSet<Admin>> _mockAdmins;
        private readonly Mock<AccountHelper> _mockAccountHelper;
        private readonly Mock<IStaffEmailGenerator> _mockEmailGenerator;
        private readonly AdminRepository _repository;
        private readonly List<Admin> _adminData;

        public AdminRepositoryTests()
        {
            _mockContext = new Mock<ApplicationDbContext>();
            _mockAdmins = new Mock<DbSet<Admin>>();
            _mockAccountHelper = new Mock<AccountHelper>();
            _mockEmailGenerator = new Mock<IStaffEmailGenerator>();

            _adminData = new List<Admin>();
            SetupMockDbSet();

            _mockContext.Setup(c => c.Admins).Returns(_mockAdmins.Object);
            _repository = new AdminRepository(_mockContext.Object, _mockAccountHelper.Object, _mockEmailGenerator.Object);
        }

        private void SetupMockDbSet()
        {
            var queryable = _adminData.AsQueryable();
            _mockAdmins.As<IQueryable<Admin>>().Setup(m => m.Provider).Returns(queryable.Provider);
            _mockAdmins.As<IQueryable<Admin>>().Setup(m => m.Expression).Returns(queryable.Expression);
            _mockAdmins.As<IQueryable<Admin>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            _mockAdmins.As<IQueryable<Admin>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            // Explicitly mock FirstOrDefaultAsync for IQueryable<Admin>
            _mockAdmins.As<IQueryable<Admin>>().Setup(m => m.FirstOrDefaultAsync(It.IsAny<Expression<Func<Admin, bool>>>(), It.IsAny<CancellationToken>()))
                .Returns<Expression<Func<Admin, bool>>, CancellationToken>((predicate, token) =>
                {
                    return Task.FromResult(_adminData.AsQueryable().FirstOrDefault(predicate));
                });

            // Explicitly mock AnyAsync for IQueryable<Admin>
            _mockAdmins.As<IQueryable<Admin>>().Setup(m => m.AnyAsync(It.IsAny<Expression<Func<Admin, bool>>>(), It.IsAny<CancellationToken>()))
                .Returns<Expression<Func<Admin, bool>>, CancellationToken>((predicate, token) =>
                {
                    return Task.FromResult(_adminData.AsQueryable().Any(predicate));
                });
        }

        [Fact]
        public async Task CreateAsync_WhenAdminDoesNotExist_CreatesAdminAndAccount()
        {
            // Arrange
            var admin = new Admin { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" };
            var password = "password123";
            var username = "john.doe@hospital.com";

            _mockEmailGenerator.Setup(g => g.GenerateEmailAsync(admin, Constants.StaffEmailDomain))
                .ReturnsAsync(username);
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1)
                .Callback(() => _adminData.Add(admin));

            // Act
            await _repository.CreateAsync(admin, password);

            // Assert
            _mockEmailGenerator.Verify(g => g.GenerateEmailAsync(admin, Constants.StaffEmailDomain), Times.Once());
            _mockAccountHelper.Verify(a => a.CreateAsync(admin, Constants.AuthRoles.Admin, username, password), Times.Once());
            _mockAdmins.Verify(m => m.AddAsync(admin, It.IsAny<CancellationToken>()), Times.Once());
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            Assert.Contains(admin, _adminData);
        }

        [Fact]
        public async Task CreateAsync_WhenAdminExists_ThrowsException()
        {
            // Arrange
            var admin = new Admin { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" };
            _adminData.Add(admin); // Simulate existing admin

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _repository.CreateAsync(admin, "password123"));
            _mockAdmins.Verify(m => m.AddAsync(It.IsAny<Admin>(), It.IsAny<CancellationToken>()), Times.Never());
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task DeleteAsync_RemovesAdminAndAccount()
        {
            // Arrange
            var admin = new Admin { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" };
            _adminData.Add(admin);
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1)
                .Callback(() => _adminData.Remove(admin));

            // Act
            await _repository.DeleteAsync(admin);

            // Assert
            _mockAdmins.Verify(m => m.Remove(admin), Times.Once());
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            _mockAccountHelper.Verify(a => a.DeleteAsync(admin), Times.Once());
            Assert.DoesNotContain(admin, _adminData);
        }

        [Fact]
        public async Task FindByIdAsync_WhenAdminExists_ReturnsAdmin()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var admin = new Admin { Id = adminId, FirstName = "John", LastName = "Doe" };
            _adminData.Add(admin);

            // Act
            var result = await _repository.FindByIdAsync(adminId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(adminId, result.Id);
        }

        [Fact]
        public async Task FindByIdAsync_WhenAdminDoesNotExist_ReturnsNull()
        {
            // Arrange
            var adminId = Guid.NewGuid();

            // Act
            var result = await _repository.FindByIdAsync(adminId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesAdminAndAccount()
        {
            // Arrange
            var admin = new Admin { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" };
            _adminData.Add(admin);
            var mockEntry = new Mock<EntityEntry<Admin>>(admin);
            mockEntry.Setup(e => e.State).Returns(EntityState.Unchanged);
            _mockContext.Setup(c => c.Entry(admin)).Returns(mockEntry.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            await _repository.UpdateAsync(admin);

            // Assert
            mockEntry.VerifySet(e => e.State = EntityState.Modified, Times.Once());
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            _mockAccountHelper.Verify(a => a.UpdateAsync(admin), Times.Once());
        }
    }
}
