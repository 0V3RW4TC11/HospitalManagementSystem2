using Commands.Patient;
using FluentValidation;

namespace Validation.Patient
{
    internal class PatientValidator : AbstractValidator<PatientBaseCommand>
    {
        public PatientValidator()
        {
            RuleFor(c => c).NotNull().WithMessage("Patient details are required");
            RuleFor(c => c.FirstName).NotEmpty().WithMessage("First name is required");
            RuleFor(c => c.Gender).NotEmpty().WithMessage("Gender is required");
            RuleFor(c => c.Email).NotEmpty().WithMessage("Email is required");
        }
    }
}