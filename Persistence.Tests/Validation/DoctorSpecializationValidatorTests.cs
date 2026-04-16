using Abstractions;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using Validation.Doctor;

namespace Tests.Validation;

[TestFixture]
internal class DoctorSpecializationValidatorTests
{
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private DoctorSpecializationValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _unitOfWorkMock = new();
        _validator = new(_unitOfWorkMock.Object);
    }

    [Test]
    public async Task Validate_SpecializationIdsIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var specIds = new List<Guid>();

        _unitOfWorkMock.Setup(u => u.Specializations.CountAsync(It.IsAny<Specifications.Entity.EntityByIdsSpec<Domain.Entities.Specialization>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act & Assert
        var result = await _validator.TestValidateAsync(specIds);
        result.ShouldHaveValidationErrorFor(x => x).WithErrorMessage("Specializations are required.");
    }

    [Test]
    public async Task Validate_SomeSpecializationIdsDoNotExist_ShouldHaveValidationError()
    {
        // Arrange
        var specIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        _unitOfWorkMock.Setup(u => u.Specializations.CountAsync(It.IsAny<Specifications.Entity.EntityByIdsSpec<Domain.Entities.Specialization>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1); // Only one exists

        // Act & Assert
        var result = await _validator.TestValidateAsync(specIds);
        result.ShouldHaveValidationErrorFor(x => x).WithErrorMessage("One or more Specializations do not exist.");
    }

    [Test]
    public async Task Validate_AllSpecializationIdsExist_ShouldNotHaveValidationError()
    {
        // Arrange
        var specIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        _unitOfWorkMock.Setup(u => u.Specializations.CountAsync(It.IsAny<Specifications.Entity.EntityByIdsSpec<Domain.Entities.Specialization>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(2); // All exist

        // Act & Assert
        var result = await _validator.TestValidateAsync(specIds);
        result.ShouldNotHaveAnyValidationErrors();
    }
}