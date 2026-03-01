using MediatR;

namespace Commands.Specialization
{
    public record UpdateSpecializationCommand(Guid Id, string Name) : IRequest;
}