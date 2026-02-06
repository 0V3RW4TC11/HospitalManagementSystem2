using Abstractions;
using Commands.Attendance;
using FluentValidation;
using Specifications.Attendance;
using Validation.Shared;

namespace Validation.Attendance
{
    public class UpdateAttendanceValidator : AbstractValidator<UpdateAttendanceCommand>
    {
        private static readonly TimeSpan _dateTolerance = TimeSpan.FromHours(6);
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAttendanceValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c).SetValidator(new AttendanceValidator());
            RuleFor(c => c.DoctorId).SetValidator(new EntityExistenceValidator<Domain.Entities.Doctor>(_unitOfWork.Doctors));
            RuleFor(c => c.PatientId).SetValidator(new EntityExistenceValidator<Domain.Entities.Patient>(_unitOfWork.Patients));
            RuleFor(c => c).MustAsync(MustBeUniqueAsync).WithMessage("An Attendance with similar details already exists.");
        }

        private async Task<bool> MustBeUniqueAsync(UpdateAttendanceCommand command, CancellationToken ct)
        {
            var dateMin = command.DateTime.AddHours(-_dateTolerance.TotalHours);
            var dateMax = command.DateTime.AddHours(_dateTolerance.TotalHours);

            var result = await _unitOfWork.Attendances.SingleOrDefaultAsync(new AttendanceByDoctorPatientDateRangeSpec(
                command.DoctorId,
                command.PatientId,
                dateMin,
                dateMax), ct);

            return result == null || result.Id == command.Id;
        }
    }
}
