using FluentValidation;
using Validation.Admin;

namespace Commands.Admin.CreateAdmin
{
    // need query to check for existing admin with same email
    public class CreateAdminValidator : AbstractValidator<CreateAdminCommand>
    {
        public CreateAdminValidator()
        {
            RuleFor(c => c.Dto)
                .NotNull()
                .SetValidator(new AdminDtoValidator());
            RuleFor(c => c.Password).NotEmpty();
        }
    }
}
