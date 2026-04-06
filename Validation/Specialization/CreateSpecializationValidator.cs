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
            RuleFor(c => c.Name).NotEmpty().WithMessage("Name is required.");
            RuleFor(c => c.Name).MustAsync(NameMustBeUniqueForThisSpecialization).WithMessage("This name is already used by another Specialization.");
        }

        private async Task<bool> NameMustBeUniqueForThisSpecialization(string name, CancellationToken ct)
        {
            return !await _unitOfWork.Specializations.AnyAsync(new SpecializationByNameSpec(name), ct);
        }
    }
}