using Abstractions;
using Commands.Patient;
using FluentValidation;

namespace Validation.Patient
{
    public class DeletePatientValidator : AbstractValidator<DeletePatientCommand>
    {
        public DeletePatientValidator(IUnitOfWork unitOfWork)
        {
            // Correctness
            RuleFor(c => c.Id).NotEmpty().WithMessage("Id is required.");

            // Existence
            RuleFor(c => c.Id)
                .SetValidator(new PatientExistenceValidator(unitOfWork));
        }
    }
}
