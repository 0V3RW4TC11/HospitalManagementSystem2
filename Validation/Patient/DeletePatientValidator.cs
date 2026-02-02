using Abstractions;
using Commands.Patient;
using FluentValidation;
using Validation.Shared;

namespace Validation.Patient
{
    public class DeletePatientValidator : AbstractValidator<DeletePatientCommand>
    {
        public DeletePatientValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(c => c.Id).SetValidator(new EntityExistenceValidator<Domain.Entities.Patient>(unitOfWork.Patients));
        }
    }
}