using DataTransfer.Specialization;

namespace DataTransfer.Doctor;

public class DoctorDto : DoctorBaseDto
{
    public Guid Id { get; set; }
    
    public IEnumerable<SpecializationDto> Specializations { get; set; }
}