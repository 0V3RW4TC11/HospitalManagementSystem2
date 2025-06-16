using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace HospitalManagementSystem2.Models.Entities
{
    public abstract class Staff
    {
        [Key]
        [ForeignKey(nameof(Account))]
        public Guid AccountId { get; set; }
        public Account Account { get; set; }

        public static Expression<Func<Staff, bool>> Matches(Staff staff)
        {
            return x => x.Account.Person.Email == staff.Account.Person.Email;
        }
    }
}
