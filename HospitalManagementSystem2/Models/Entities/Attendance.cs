using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Migrations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem2.Models.Entities
{
    public class Attendance
    {
        [Key]
        [ForeignKey(nameof(Appointment))]
        public Guid AppointmentId { get; set; }
        public Appointment Appointment { get; set; }
        public string Diagnosis { get; set; }
        public string Remarks { get; set; }
        public string Therapy { get; set; }
    }
}
