using Abstractions;
using Commands.Admin;
using FluentValidation;
using Validation.Entity;

namespace Validation.Admin
{
    public class DeleteAdminValidator : AbstractValidator<DeleteAdminCommand>
    {
        public DeleteAdminValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(c => c.Id).SetValidator(new EntityExistenceValidator<Entities.Admin>(unitOfWork.Admins));
        }
    }
}