using Ardalis.Specification;

namespace Specifications.Entity
{
    public class EntityByIdSpec<T> : SingleResultSpecification<T> where T : Domain.Entities.Entity
    {
        public EntityByIdSpec(Guid id)
        {
            Query.Where(x => x.Id == id);
        }
    }
}
