using Abstractions;
using Commands.Admin;
using FluentValidation;

namespace Validation.Admin
{
    public class DeleteAdminValidator : AbstractValidator<DeleteAdminCommand>
    {
        public DeleteAdminValidator(IUnitOfWork unitOfWork)
        {
            // Correctness
            RuleFor(c => c.Id).NotEmpty().WithMessage("Id is required.");

            // Existence
            RuleFor(c => c.Id)
                .SetValidator(new AdminExistenceValidator(unitOfWork));
        }
    }
}
