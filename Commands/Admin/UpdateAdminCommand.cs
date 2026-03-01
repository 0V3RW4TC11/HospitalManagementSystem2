using MediatR;

namespace Commands.Admin
{
    public record UpdateAdminCommand(
        Guid Id,
        string? Title,
        string FirstName,
        string? LastName,
        string? Gender,
        string? Address,
        string Phone,
        string Email,
        DateOnly? DateOfBirth) : IRequest;
}