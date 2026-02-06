using Commands.Admin;
using FluentValidation;

namespace Validation.Admin
{
    internal class AdminValidator : AbstractValidator<AdminBaseCommand>
    {
        public AdminValidator()
        {
            RuleFor(c => c).NotNull().WithMessage("Admin details are required.");
            RuleFor(c => c.FirstName).NotEmpty().WithMessage("First name is required");
            RuleFor(c => c.Phone).NotEmpty().WithMessage("Phone number is required");
            RuleFor(c => c.Email).NotEmpty().WithMessage("Email is required");
        }
    }
}