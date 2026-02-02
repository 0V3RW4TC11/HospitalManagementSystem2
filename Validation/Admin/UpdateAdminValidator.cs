using Abstractions;
using Commands.Admin;
using FluentValidation;
using Specifications.Admin;
using Validation.Shared;

namespace Validation.Admin
{
    // TODO: use DB contraint in favor of uniqueness check to avoid potential race condition
    public class UpdateAdminValidator : AbstractValidator<UpdateAdminCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAdminValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            // Correctness
            RuleFor(c => c).SetValidator(new AdminCorrectnessValidator());
            RuleFor(c => c.Id).SetValidator(new EntityExistenceValidator<Domain.Entities.Admin>(_unitOfWork.Admins));
            RuleFor(c => c.Email).MustAsync(EmailMustBeUniqueForThisAdmin).WithMessage("This email is already used by another Admin");
        }

        private async Task<bool> EmailMustBeUniqueForThisAdmin(UpdateAdminCommand command, string email, CancellationToken cancellationToken)
        {
            Guid idFromEmail = await _unitOfWork.Admins.SingleOrDefaultAsync(new AdminIdByEmailSpec(email), cancellationToken);

            return (idFromEmail == Guid.Empty) || (idFromEmail == command.Id);
        }
    }
}