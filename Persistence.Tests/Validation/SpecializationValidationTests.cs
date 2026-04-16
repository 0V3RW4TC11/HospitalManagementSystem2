using Abstractions;
using Commands.Specialization;
using FluentValidation.TestHelper;
using Moq;
using Validation.Specialization;

namespace Tests.Validation
{
    [TestFixture]
    internal class SpecializationValidationTests
    {
        private readonly SpecializationValidator _specValidator = new();

        private static void CreateUowAndRepoMock(out Mock<IUnitOfWork> uowMock, out Mock<IRepository<Domain.Entities.Specialization>> repoMock)
        {
            uowMock = new();
            repoMock = new();
            uowMock.Setup(u => u.Specializations).Returns(repoMock.Object);
        }

        [Test]
        public void SpecializationValidator_NameIsEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var command = new CreateSpecializationCommand("");

            // Act

			// Assert
            var result = _specValidator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Name).WithErrorMessage("Name is required.");
        }

        [Test]
        public void SpecializationValidator_NameIsNotEmpty_ShouldNotHaveValidationErrors()
        {
            // Arrange
            var command = new CreateSpecializationCommand("Cardiology");

            // Act

			// Assert
            var result = _specValidator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public async Task CreateValidator_NameIsNotUnique_ShouldHaveValidationError()
        {
            // Arrange
            var command = new CreateSpecializationCommand("Existing Name");
            var uowMock = new Mock<IUnitOfWork>();
            uowMock.Setup(u => u.Specializations.AnyAsync(It.IsAny<Specifications.Specialization.SpecializationByNameSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            var validator = new CreateSpecializationValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name).WithErrorMessage("This name is already used by another Specialization.");
        }

        [Test]
        public async Task CreateValidator_NameIsUnique_ShouldNotHaveValidationError()
        {
            // Arrange
            var command = new CreateSpecializationCommand("Unique Name");
            var uowMock = new Mock<IUnitOfWork>();
            uowMock.Setup(u => u.Specializations.AnyAsync(It.IsAny<Specifications.Specialization.SpecializationByNameSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            var validator = new CreateSpecializationValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Test]
        public async Task CreateValidator_NameIsEmpty_ShouldHaveValidationError()
        {
            // Arrange
            var command = new CreateSpecializationCommand("");
            var uowMock = new Mock<IUnitOfWork>();
            uowMock.Setup(u => u.Specializations.AnyAsync(It.IsAny<Specifications.Specialization.SpecializationByNameSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            var validator = new CreateSpecializationValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name).WithErrorMessage("Name is required.");
        }

        [Test]
        public async Task UpdateValidator_NameIsUsedByAnotherSpecialization_ShouldHaveValidationError()
        {
            // Arrange
            var command = new UpdateSpecializationCommand(Guid.NewGuid(), "Existing Name");
            CreateUowAndRepoMock(out var uowMock, out var repoMock);
            repoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Specialization>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            uowMock.Setup(u => u.Specializations.SingleOrDefaultAsync(It.IsAny<Specifications.Specialization.SpecializationIdByNameSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid()); // Different id
            var validator = new UpdateSpecializationValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x).WithErrorMessage("This name is already used by another Specialization.");
        }

        [Test]
        public async Task UpdateValidator_NameIsUsedBySameSpecialization_ShouldNotHaveValidationError()
        {
            // Arrange
            var specializationId = Guid.NewGuid();
            var command = new UpdateSpecializationCommand(specializationId, "Existing Name");
            CreateUowAndRepoMock(out var uowMock, out var repoMock);
            repoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Specialization>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            uowMock.Setup(u => u.Specializations.SingleOrDefaultAsync(It.IsAny<Specifications.Specialization.SpecializationIdByNameSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(specializationId); // Same id
            var validator = new UpdateSpecializationValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x);
        }

        [Test]
        public async Task UpdateValidator_NameIsUnique_ShouldNotHaveValidationError()
        {
            // Arrange
            var command = new UpdateSpecializationCommand(Guid.NewGuid(), "Unique Name");
            CreateUowAndRepoMock(out var uowMock, out var repoMock);
            repoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Specialization>>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(true);
            uowMock.Setup(u => u.Specializations.SingleOrDefaultAsync(It.IsAny<Specifications.Specialization.SpecializationIdByNameSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.Empty); // Not used
            var validator = new UpdateSpecializationValidator(uowMock.Object);

            // Act
            var result = await validator.TestValidateAsync(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x);
        }
    }
}
