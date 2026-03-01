using MediatR;

namespace Commands.Specialization
{
    public record CreateSpecializationCommand(string Name) : IRequest;
}