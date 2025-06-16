using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem2.Models.Entities
{
    public class Guest : Entity
    {
        public Guid PersonId { get; set; }
        public Person Person { get; set; }
    }
}
