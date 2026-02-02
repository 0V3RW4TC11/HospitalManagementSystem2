using Commands.Specialization;
using FluentValidation;

namespace Validation.Specialization
{
    internal class SpecializationValidator : AbstractValidator<CreateSpecializationCommand>
    {
        public SpecializationValidator()
        {
            RuleFor(c => c.Name).NotEmpty().WithMessage("Name is required.");
        }
    }
}