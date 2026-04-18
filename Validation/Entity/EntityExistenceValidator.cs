using Abstractions;
using FluentValidation;
using Specifications.Entity;

namespace Validation.Entity
{
    internal class EntityExistenceValidator<TEntity> : AbstractValidator<Guid> where TEntity : Entities.Entity
    {
        private readonly IRepository<TEntity> _repository;

        public EntityExistenceValidator(IRepository<TEntity> repository)
        {
            _repository = repository;

            RuleFor(id => id)
                .NotEmpty().WithMessage("Id is required.")
                .MustAsync(EntityMustExistWithId).WithMessage(typeof(TEntity).Name + " with this Id does not exist.");
        }

        private async Task<bool> EntityMustExistWithId(Guid id, CancellationToken cancellationToken)
        {
            return await _repository.AnyAsync(new EntityByIdSpec<TEntity>(id), cancellationToken);
        }
    }
}