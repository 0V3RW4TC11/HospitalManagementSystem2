using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem2.Models.Entities
{
    [PrimaryKey(nameof(DoctorId), nameof(SpecializationId))]
    public class DoctorSpecialization
    {
        //[Key]
        public Guid DoctorId { get; set; }

        public Doctor Doctor { get; set; }

        //[Key]
        public Guid SpecializationId { get; set; }

        public Specialization Specialization { get; set; }
    }
}
