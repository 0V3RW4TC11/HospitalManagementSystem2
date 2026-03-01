using MediatR;

namespace Commands.Admin
{
    public record CreateAdminCommand(
        string? Title,
        string FirstName,
        string? LastName,
        string? Gender,
        string? Address,
        string Phone,
        string Email,
        string Password,
        DateOnly? DateOfBirth) : IRequest;
}