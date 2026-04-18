using Ardalis.Specification;

namespace Specifications.Entity
{
    public class EntityByIdsSpec<TEntity> : Specification<TEntity> where TEntity : Entities.Entity
    {
        public EntityByIdsSpec(IEnumerable<Guid> ids)
        {
            Query.Where(e => ids.Contains(e.Id));
        }
    }
}