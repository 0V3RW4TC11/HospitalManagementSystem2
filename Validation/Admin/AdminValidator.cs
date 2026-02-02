using Commands.Admin;
using FluentValidation;

namespace Validation.Admin
{
    internal class AdminValidator : AbstractValidator<AdminBaseCommand>
    {
        public AdminValidator()
        {
            RuleFor(a => a).NotNull().WithMessage("Admin details are required.");
            RuleFor(a => a.FirstName).NotEmpty().WithMessage("First name is required");
            RuleFor(a => a.Phone).NotEmpty().WithMessage("Phone number is required");
            RuleFor(a => a.Email).NotEmpty().WithMessage("Email is required");
        }
    }
}