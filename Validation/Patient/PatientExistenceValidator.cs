using Abstractions;
using FluentValidation;
using Specifications.Entity;

namespace Validation.Patient
{
    internal class PatientExistenceValidator : AbstractValidator<Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public PatientExistenceValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(id => id)
                .MustAsync(PatientMustExistWithId)
                .WithMessage("Patient with this Id does not exist.");
        }

        private async Task<bool> PatientMustExistWithId(Guid id, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Patients.AnyAsync(new EntityByIdSpec<Domain.Entities.Patient>(id), cancellationToken);
        }
    }
}
