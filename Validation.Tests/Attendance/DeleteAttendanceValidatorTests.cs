using Abstractions;
using Commands.Attendance;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using Validation.Attendance;

namespace Validation.Tests.Attendance;

[TestFixture]
public class DeleteAttendanceValidatorTests
{
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<IRepository<Domain.Entities.Attendance>> _attendanceRepoMock;
    private DeleteAttendanceValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _attendanceRepoMock = new Mock<IRepository<Domain.Entities.Attendance>>();
        _unitOfWorkMock.Setup(u => u.Attendances).Returns(_attendanceRepoMock.Object);
        _validator = new DeleteAttendanceValidator(_unitOfWorkMock.Object);
    }

    [Test]
    public async Task Validate_IdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new DeleteAttendanceCommand(Guid.Empty);

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Id).WithErrorMessage("Id is required.");
    }

    [Test]
    public async Task Validate_AttendanceDoesNotExist_ShouldHaveValidationError()
    {
        // Arrange
        var command = new DeleteAttendanceCommand(Guid.NewGuid());

        _attendanceRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Attendance>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Id).WithErrorMessage("Attendance with this Id does not exist.");
    }

    [Test]
    public async Task Validate_AttendanceExists_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new DeleteAttendanceCommand(Guid.NewGuid());

        _attendanceRepoMock.Setup(r => r.AnyAsync(It.IsAny<Specifications.Entity.EntityByIdSpec<Domain.Entities.Attendance>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}