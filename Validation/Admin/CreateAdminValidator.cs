using Abstractions;
using FluentValidation;
using Specifications.Admin;
using Validation.Admin;

namespace Commands.Admin.CreateAdmin
{
    // TODO: use DB contraint in favor of uniqueness check to avoid potential race condition
    public class CreateAdminValidator : AbstractValidator<CreateAdminCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateAdminValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c).SetValidator(new AdminValidator());
            RuleFor(c => c.Password).NotEmpty().WithMessage("Password is required.");
            RuleFor(c => c.Email).MustAsync(EmailMustBeUniqueForThisAdmin).WithMessage("This email is already used by another Admin");
        }

        private async Task<bool> EmailMustBeUniqueForThisAdmin(string email, CancellationToken ct)
        {
            return !await _unitOfWork.Admins.AnyAsync(new AdminByEmailSpec(email), ct);
        }
    }
}