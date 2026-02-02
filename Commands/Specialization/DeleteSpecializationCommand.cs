using MediatR;

namespace Commands.Specialization
{
    public record DeleteSpecializationCommand(Guid Id) : IRequest;
}