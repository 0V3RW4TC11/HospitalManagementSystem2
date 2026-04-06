using Abstractions;
using Commands.Attendance;
using FluentValidation;

namespace Validation.Attendance
{
    public class UpdateAttendanceValidator : AbstractValidator<UpdateAttendanceCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAttendanceValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.Data).SetValidator(new AttendanceValidator(unitOfWork));
            RuleFor(c => c).MustAsync(MustBeUniqueAsync).WithMessage("An Attendance with similar details already exists.");
        }

        private async Task<bool> MustBeUniqueAsync(UpdateAttendanceCommand command, CancellationToken ct)
        {
            var result = await AttendanceValidationHelper.GetAttendanceInDateRangeAsync(command.Data, _unitOfWork, ct);

            return result == null || result.Id == command.Id;
        }
    }
}
