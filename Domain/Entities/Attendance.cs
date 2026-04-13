namespace Domain.Entities;

public class Attendance : Entity
{
    public Guid PatientId { get; set; }

    public Guid DoctorId { get; set; }

    public DateTime DateTime { get; set; }

    public string Diagnosis { get; set; }

    public string Remarks { get; set; }
    
    public string Therapy { get; set; }
}