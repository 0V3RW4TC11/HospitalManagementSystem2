using MediatR;

namespace Commands.Doctor
{
    public record DoctorBaseCommand : IRequest
    {
        public string FirstName { get; init; }

        public string LastName { get; init; }

        public string Gender { get; init; }

        public string Address { get; init; }

        public string Phone { get; init; }

        public string Email { get; init; }

        public DateOnly? DateOfBirth { get; init; }

        public IEnumerable<Guid> SpecializationIds { get; init; }
    }
}