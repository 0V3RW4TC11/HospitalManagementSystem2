using Abstractions;
using Commands.Specialization;
using FluentValidation;
using Validation.Entity;

namespace Validation.Specialization
{
    public class DeleteSpecializationValidator : AbstractValidator<DeleteSpecializationCommand>
    {
        public DeleteSpecializationValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(c => c.Id).SetValidator(new EntityExistenceValidator<Domain.Entities.Specialization>(unitOfWork.Specializations));
        }
    }
}