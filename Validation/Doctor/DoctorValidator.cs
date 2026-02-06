using Commands.Doctor;
using FluentValidation;

namespace Validation.Doctor
{
    internal class DoctorValidator : AbstractValidator<DoctorBaseCommand>
    {
        public DoctorValidator()
        {
            RuleFor(c => c).NotNull().WithMessage("Doctor details are required.");
            RuleFor(c => c.FirstName).NotEmpty().WithMessage("First name is required");
            RuleFor(c => c.Phone).NotEmpty().WithMessage("Phone number is required");
            RuleFor(c => c.Email).NotEmpty().WithMessage("Email is required");
        }
    }
}