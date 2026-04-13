namespace Commands.Doctor
{
    public record DoctorData(
        string FirstName,
        string LastName,
        string Gender,
        string Address,
        string Phone,
        string Email,
        DateOnly DateOfBirth,
        IEnumerable<Guid> SpecializationIds);
}
