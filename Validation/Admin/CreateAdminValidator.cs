using Abstractions;
using Commands.Admin;
using FluentValidation;
using Specifications.Admin;

namespace Validation.Admin
{
    public class CreateAdminValidator : AbstractValidator<CreateAdminCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateAdminValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.Data).SetValidator(new AdminValidator());
            RuleFor(c => c.Password).NotEmpty().WithMessage("Password is required.");
            RuleFor(c => c.Data.Email).MustAsync(EmailMustBeUniqueForThisAdmin).WithMessage("This email is already used by another Admin");
        }

        // TODO: use DB contraint in favor of uniqueness check to avoid potential race condition
        private async Task<bool> EmailMustBeUniqueForThisAdmin(string email, CancellationToken ct)
        {
            return !await _unitOfWork.Admins.AnyAsync(new AdminByEmailSpec(email), ct);
        }
    }
}