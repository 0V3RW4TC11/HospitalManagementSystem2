using System.Linq.Expressions;

namespace Domain.Entities;

public class Specialization : Entity
{
    public string Name { get; set; }

    public static Expression<Func<Specialization, bool>> Matches(Specialization spec)
    {
        return x => x.Name.ToLower() == spec.Name.ToLower();
    }
}