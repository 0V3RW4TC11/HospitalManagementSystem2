namespace Services.Dtos.Doctor;

public class DoctorCreateDto : DoctorBaseDto
{
    public ISet<Guid> SpecializationIds { get; set; }

    public string Password { get; set; }
}