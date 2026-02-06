using Commands.Attendance;
using FluentValidation;

namespace Validation.Attendance
{
    internal class AttendanceValidator : AbstractValidator<CreateAttendanceCommand>
    {
        public AttendanceValidator()
        {
            RuleFor(c => c.Diagnosis).NotEmpty().WithMessage("Diagnosis is required.");
            RuleFor(c => c.Remarks).NotEmpty().WithMessage("Remarks is required.");
            RuleFor(c => c.Therapy).NotEmpty().WithMessage("Therapy is required.");
            RuleFor(c => c.DateTime).Must(DateMustBeInPast).WithMessage("Date cannot be in the future.");
        }

        private static bool DateMustBeInPast(DateTime date)
        {
            return date <= DateTime.UtcNow;
        }
    }
}
