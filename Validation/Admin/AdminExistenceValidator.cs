using Abstractions;
using FluentValidation;
using Specifications.Entity;

namespace Validation.Admin
{
    internal class AdminExistenceValidator : AbstractValidator<Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminExistenceValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(id => id)
                .MustAsync(AdminMustExistWithId)
                .WithMessage("Admin with this Id does not exist.");
        }

        private async Task<bool> AdminMustExistWithId(Guid id, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Admins.AnyAsync(new EntityByIdSpec<Domain.Entities.Admin>(id), cancellationToken);
        }
    }
}
