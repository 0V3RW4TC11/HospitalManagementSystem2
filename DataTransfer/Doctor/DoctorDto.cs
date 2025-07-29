namespace DataTransfer.Doctor;

public class DoctorDto : DoctorBaseDto
{
    public Guid Id { get; set; }
    
    public IEnumerable<Guid> SpecializationIds { get; set; }
}