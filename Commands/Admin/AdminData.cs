namespace Commands.Admin
{
    public record AdminData(
        string? Title,
        string FirstName,
        string? LastName,
        string? Gender,
        string? Address,
        string Phone,
        string Email,
        DateOnly? DateOfBirth);
}
