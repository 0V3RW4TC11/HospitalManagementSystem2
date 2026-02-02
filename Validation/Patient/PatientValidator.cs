using Commands.Patient;
using FluentValidation;

namespace Validation.Patient
{
    internal class PatientValidator : AbstractValidator<PatientBaseCommand>
    {
        public PatientValidator()
        {
            RuleFor(p => p).NotNull().WithMessage("Patient details are required");
            RuleFor(p => p.FirstName).NotEmpty().WithMessage("First name is required");
            RuleFor(p => p.Gender).NotEmpty().WithMessage("Gender is required");
            RuleFor(p => p.Email).NotEmpty().WithMessage("Email is required");
        }
    }
}