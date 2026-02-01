using Abstractions;
using Commands.Admin;
using FluentValidation;
using Validation.Shared;

namespace Validation.Admin
{
    public class DeleteAdminValidator : AbstractValidator<DeleteAdminCommand>
    {
        public DeleteAdminValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(c => c.Id).SetValidator(new EntityValidator<Domain.Entities.Admin>(unitOfWork.Admins));
        }
    }
}
