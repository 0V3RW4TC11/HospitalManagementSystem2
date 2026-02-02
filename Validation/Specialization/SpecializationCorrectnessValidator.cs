using Commands.Specialization;
using FluentValidation;

namespace Validation.Specialization
{
    internal class SpecializationCorrectnessValidator : AbstractValidator<CreateSpecializationCommand>
    {
        public SpecializationCorrectnessValidator()
        {
            RuleFor(c => c.Name).NotEmpty().WithMessage("Name is required.");
        }
    }
}