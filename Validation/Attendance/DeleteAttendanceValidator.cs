using Abstractions;
using Commands.Attendance;
using FluentValidation;
using Validation.Shared;

namespace Validation.Attendance
{
    public class DeleteAttendanceValidator : AbstractValidator<DeleteAttendanceCommand>
    {
        public DeleteAttendanceValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(c => c.Id).SetValidator(new EntityExistenceValidator<Domain.Entities.Attendance>(unitOfWork.Attendances));
        }
    }
}
