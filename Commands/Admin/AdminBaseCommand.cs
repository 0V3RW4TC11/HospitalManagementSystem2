using MediatR;

namespace Commands.Admin
{
    public abstract record AdminBaseCommand(
        string? Title,
        string FirstName,
        string? LastName,
        string? Gender,
        string? Address,
        string Phone,
        string Email,
        DateOnly? DateOfBirth) : IRequest;
}