using Abstractions;
using Commands.Attendance;
using FluentValidation;
using Validation.Shared;

namespace Validation.Attendance
{
    internal class AttendanceValidator : AbstractValidator<AttendanceData>
    {
        public AttendanceValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(c => c.Diagnosis).NotEmpty().WithMessage("Diagnosis is required.");
            RuleFor(c => c.Remarks).NotEmpty().WithMessage("Remarks is required.");
            RuleFor(c => c.Therapy).NotEmpty().WithMessage("Therapy is required.");
            RuleFor(c => c.DateTime).Must(DateMustBeInPast).WithMessage("Date cannot be in the future.");
            RuleFor(c => c.DoctorId).SetValidator(new EntityExistenceValidator<Domain.Entities.Doctor>(unitOfWork.Doctors));
            RuleFor(c => c.PatientId).SetValidator(new EntityExistenceValidator<Domain.Entities.Patient>(unitOfWork.Patients));
        }

        private static bool DateMustBeInPast(DateTime date)
        {
            return date <= DateTime.UtcNow;
        }
    }
}
