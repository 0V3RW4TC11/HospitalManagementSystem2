using Abstractions;
using Commands.Specialization;
using FluentValidation;
using Specifications.Specialization;

namespace Validation.Specialization
{
    public class CreateSpecializationValidator : AbstractValidator<CreateSpecializationCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateSpecializationValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c).SetValidator(new SpecializationCorrectnessValidator());
            RuleFor(c => c.Name).MustAsync(NameMustBeUniqueForThisSpecialization).WithMessage("This name is already used by another Specialization.");
        }

        private async Task<bool> NameMustBeUniqueForThisSpecialization(CreateSpecializationCommand command, string name, CancellationToken ct)
        {
            return !await _unitOfWork.Specializations.AnyAsync(new SpecializationByNameSpec(name), ct);
        }
    }
}