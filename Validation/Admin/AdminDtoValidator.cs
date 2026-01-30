using Dtos.Admin;
using FluentValidation;

namespace Validation.Admin
{
    public class AdminDtoValidator : AbstractValidator<AdminDto>
    {
        public AdminDtoValidator()
        {
            RuleFor(a => a.FirstName).NotEmpty();
            RuleFor(a => a.Phone).NotEmpty();
            RuleFor(a => a.Email).NotEmpty();
        }
    }
}
