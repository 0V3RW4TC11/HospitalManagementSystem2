using MediatR;

namespace Commands.Patient
{
    public record PatientBaseCommand : IRequest
    {
        public string? Title { get; init; }

        public string FirstName { get; init; }

        public string? LastName { get; init; }

        public string Gender { get; init; }

        public string? Address { get; init; }

        public string? Phone { get; init; }

        public string Email { get; init; }

        public Constants.BloodType BloodType { get; init; }

        public DateOnly DateOfBirth { get; init; }
    }
}