namespace DataTransfer.Patient;

public abstract class PatientBaseDto
{
    public string? Title { get; set; }

    public string FirstName { get; set; }

    public string? LastName { get; set; }

    public string Gender { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string Email { get; set; }
    
    public int BloodType { get; set; }

    public DateOnly DateOfBirth { get; set; }
}