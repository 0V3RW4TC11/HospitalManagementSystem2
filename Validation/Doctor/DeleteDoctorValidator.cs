using Abstractions;
using Commands.Doctor;
using FluentValidation;
using Validation.Entity;

namespace Validation.Doctor
{
    public class DeleteDoctorValidator : AbstractValidator<DeleteDoctorCommand>
    {
        public DeleteDoctorValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(c => c.Id).SetValidator(new EntityExistenceValidator<Entities.Doctor>(unitOfWork.Doctors));
        }
    }
}