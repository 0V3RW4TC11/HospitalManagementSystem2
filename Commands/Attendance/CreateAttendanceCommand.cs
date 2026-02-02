using MediatR;

namespace Commands.Attendance
{
    public record CreateAttendanceCommand : IRequest
    {
        public Guid PatientId { get; init; }

        public Guid DoctorId { get; init; }

        public DateTime DateTime { get; init; }

        public string Diagnosis { get; init; }

        public string Remarks { get; init; }

        public string Therapy { get; init; }
    }
}
