using MediatR;

namespace Commands.Patient
{
    public record CreatePatientCommand(
        string? Title,
        string FirstName,
        string? LastName,
        string Gender,
        string? Address,
        string? Phone,
        string Email,
        Constants.BloodType BloodType,
        DateOnly DateOfBirth) : IRequest;
}