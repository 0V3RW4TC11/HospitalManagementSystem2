using Abstractions;
using Commands.Attendance;
using FluentValidation;

namespace Validation.Attendance
{
    public class CreateAttendanceValidator : AbstractValidator<CreateAttendanceCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateAttendanceValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.Data).SetValidator(new AttendanceValidator(unitOfWork));
            RuleFor(c => c.Data).MustAsync(MustBeUniqueAsync).WithMessage("An Attendance with similar details already exists.");
        }

        private async Task<bool> MustBeUniqueAsync(AttendanceData data, CancellationToken ct)
        {
            var result = await AttendanceValidationHelper.GetAttendanceInDateRangeAsync(data, _unitOfWork, ct);

            return result == null;
        }
    }
}
