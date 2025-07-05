using System.Linq.Expressions;

namespace HospitalManagementSystem2.Models.Entities
{
    public abstract class Staff : Account
    {
        public static Expression<Func<Staff, bool>> Matches(Staff staff)
        {
            return x => 
                (x.Email == null && staff.Email == null) ||
                (x.Email != null && staff.Email != null &&
                x.Email.ToLower() == staff.Email.ToLower());
        }
    }
}
