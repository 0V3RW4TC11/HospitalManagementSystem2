using Abstractions;
using Commands.Attendance;
using Specifications.Attendance;

namespace Validation.Attendance
{
    internal static class AttendanceValidationHelper
    {
        private static readonly TimeSpan _dateTolerance = TimeSpan.FromHours(6);

        public static async Task<Entities.Attendance?> GetAttendanceInDateRangeAsync(AttendanceData data, IUnitOfWork unitOfWork, CancellationToken ct)
        {
            var dateMin = data.DateTime.AddHours(-_dateTolerance.TotalHours);
            var dateMax = data.DateTime.AddHours(_dateTolerance.TotalHours);

            return await unitOfWork.Attendances.SingleOrDefaultAsync(new AttendanceByDoctorPatientDateRangeSpec(
                data.DoctorId,
                data.PatientId,
                dateMin,
                dateMax), ct);
        }
    }
}
