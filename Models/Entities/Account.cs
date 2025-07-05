using Microsoft.AspNetCore.Identity;

namespace HospitalManagementSystem2.Models.Entities
{
    public abstract class Account : Person
    {
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
    }
}
