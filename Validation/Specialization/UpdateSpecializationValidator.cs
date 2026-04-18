using Abstractions;
using Commands.Specialization;
using FluentValidation;
using Specifications.Specialization;
using Validation.Entity;

namespace Validation.Specialization
{
    public class UpdateSpecializationValidator : AbstractValidator<UpdateSpecializationCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateSpecializationValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            RuleFor(c => c.Name).NotEmpty().WithMessage("Name is required.");
            RuleFor(c => c.Id).SetValidator(new EntityExistenceValidator<Entities.Specialization>(_unitOfWork.Specializations));
            RuleFor(c => c).MustAsync(NameMustBeUniqueForThisSpecialization).WithMessage("This name is already used by another Specialization.");
        }

        private async Task<bool> NameMustBeUniqueForThisSpecialization(Commands.Specialization.UpdateSpecializationCommand command, CancellationToken ct)
        {
            Guid idFromName = await _unitOfWork.Specializations.SingleOrDefaultAsync(new SpecializationIdByNameSpec(command.Name), ct);
            return (idFromName == Guid.Empty) || (idFromName == command.Id);
        }
    }
}