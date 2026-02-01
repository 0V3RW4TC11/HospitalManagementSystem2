using Abstractions;
using Domain.Entities;
using FluentValidation;
using Specifications.Entity;

namespace Validation.Doctor
{
    internal class DoctorSpecializationExistenceValidator : AbstractValidator<IEnumerable<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DoctorSpecializationExistenceValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(ids => ids)
                .NotEmpty().WithMessage("Specializations are required.")
                .MustAsync(AllSpecIdsMustExistAsync).WithMessage("One or more Specializations do not exist.");
        }

        private async Task<bool> AllSpecIdsMustExistAsync(IEnumerable<Guid> specIds, CancellationToken ct)
        {
            int matchingIdsCount = await _unitOfWork.Specializations.CountAsync(new EntityByIdsSpec<Specialization>(specIds), ct);
            return specIds.Count() == matchingIdsCount;
        }
    }
}
