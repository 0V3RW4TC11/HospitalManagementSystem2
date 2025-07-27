namespace DataTransfer.Doctor;

public class DoctorUpdateDto : DoctorBaseDto
{
    public Guid Id { get; set; }
    
    public ISet<Guid> SpecializationIds { get; set; }
}