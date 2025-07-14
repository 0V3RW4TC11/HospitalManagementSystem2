using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem2.Models.Entities;

[PrimaryKey(nameof(DoctorId), nameof(SpecializationId))]
public class DoctorSpecialization
{
    public Guid DoctorId { get; set; }
    public Doctor Doctor { get; set; }

    public Guid SpecializationId { get; set; }
    public Specialization Specialization { get; set; }
}