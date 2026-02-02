using Abstractions;
using FluentValidation;
using Specifications.Specialization;
using Validation.Shared;

namespace Validation.Specialization
{
    public class UpdateSpecializationCommand : AbstractValidator<Commands.Specialization.UpdateSpecializationCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateSpecializationCommand(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c).SetValidator(new SpecializationValidator());
            RuleFor(c => c.Id).SetValidator(new EntityExistenceValidator<Domain.Entities.Specialization>(_unitOfWork.Specializations));
            RuleFor(c => c.Name).MustAsync(NameMustBeUniqueForThisSpecialization).WithMessage("This name is already used by another Specialization.");
        }

        private async Task<bool> NameMustBeUniqueForThisSpecialization(Commands.Specialization.UpdateSpecializationCommand command, string name, CancellationToken ct)
        {
            Guid idFromName = await _unitOfWork.Specializations.SingleOrDefaultAsync(new SpecializationIdByNameSpec(name), ct);
            return (idFromName == Guid.Empty) || (idFromName == command.Id);
        }
    }
}