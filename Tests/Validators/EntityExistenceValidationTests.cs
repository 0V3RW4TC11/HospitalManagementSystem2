using Abstractions;
using Entities;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using Validation.Entity;

namespace Tests.Validators
{
    [TestFixture]
    internal class EntityExistenceValidationTests
    {
        private Mock<IRepository<Admin>> _adminRepoMock;
        private EntityExistenceValidator<Admin> _validator;

        [SetUp]
        public void Setup()
        {
            _adminRepoMock = new();
            _validator = new(_adminRepoMock.Object);
        }

        [Test]
        public async Task Validate_IdIsEmpty_ShouldHaveValidationError()
        {
            // Act
            var result = await _validator.TestValidateAsync(Guid.Empty);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x).WithErrorMessage("Id is required.");
        }

        [Test]
        public async Task Validate_IdDoesNotExist_ShouldHaveValidationError()
        {
            // Arrange
            _adminRepoMock.Setup(r => r
                .AnyAsync(
                    It.IsAny<Specifications.Entity.EntityByIdSpec<Admin>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _validator.TestValidateAsync(Guid.NewGuid());

            // Assert
            result.ShouldHaveValidationErrorFor(x => x).WithErrorMessage(nameof(Admin) + " with this Id does not exist.");
        }

        [Test]
        public async Task Validate_IdDoesExist_ShouldNotHaveValidationErrors()
        {
            // Arrange
            _adminRepoMock.Setup(r => r
                .AnyAsync(
                    It.IsAny<Specifications.Entity.EntityByIdSpec<Admin>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _validator.TestValidateAsync(Guid.NewGuid());

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
