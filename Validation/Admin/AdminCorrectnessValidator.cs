using Dtos.Admin;
using FluentValidation;

namespace Validation.Admin
{
    internal class AdminCorrectnessValidator : AbstractValidator<AdminDto>
    {
        public AdminCorrectnessValidator()
        {
            RuleFor(a => a).NotNull().WithMessage("Admin details are required.");
            RuleFor(a => a.FirstName).NotEmpty().WithMessage("First name is required");
            RuleFor(a => a.Phone).NotEmpty().WithMessage("Phone number is required");
            RuleFor(a => a.Email).NotEmpty().WithMessage("Email is required");
        }
    }
}
