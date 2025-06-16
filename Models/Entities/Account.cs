using Microsoft.AspNetCore.Identity;

namespace HospitalManagementSystem2.Models.Entities
{
    public class Account : Entity
    {
        public Guid PersonId { get; set; }
        public Person Person { get; set; }
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
    }
}
