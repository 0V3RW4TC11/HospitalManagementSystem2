using Abstractions;
using Domain.Entities;
using FluentValidation;
using Specifications.Entity;

namespace Validation.Shared
{
    internal class EntityValidator<TEntity> : AbstractValidator<Guid> where TEntity : Entity
    {
        private readonly IRepository<TEntity> _repository;

        public EntityValidator(IRepository<TEntity> repository)
        {
            _repository = repository;

            RuleFor(id => id)
                .NotEmpty().WithMessage("Id is required.")
                .MustAsync(EntityMustExistWithId).WithMessage(nameof(TEntity) + " with this Id does not exist.");
        }

        private async Task<bool> EntityMustExistWithId(Guid id, CancellationToken cancellationToken)
        {
            return await _repository.AnyAsync(new EntityByIdSpec<TEntity>(id), cancellationToken);
        }
    }
}
