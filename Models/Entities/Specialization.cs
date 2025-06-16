using System.Linq.Expressions;

namespace HospitalManagementSystem2.Models.Entities
{
    public class Specialization : Entity
    {
        public string Name { get; set; }

        public static Expression<Func<Specialization, bool>> Matches(Specialization spec)
        {
            return x => x.Name == spec.Name;
        }
    }
}
