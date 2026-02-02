using MediatR;

namespace Commands.Specialization
{
    public record CreateSpecializationCommand : IRequest
    {
        public string Name { get; init; }
    }
}