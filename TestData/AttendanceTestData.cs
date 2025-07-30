using DataTransfer.Attendance;
using Domain.Entities;
using Mapster;
using Persistence;

namespace TestData;

public static class AttendanceTestData
{
    public static AttendanceCreateDto CreateDto(Guid patientId, Guid doctorId) => new()
    {
        PatientId = patientId,
        DoctorId = doctorId,
        DateTime = DateTime.Now + TimeSpan.FromMinutes(5),
        Diagnosis = "Example diagnosis",
        Remarks = "Example remarks",
        Therapy = "Example therapy"
    };

    public static async Task<AttendanceDto> SeedAttendance(
        RepositoryDbContext context,
        Guid patientId,
        Guid doctorId)
    {
        var createDto = CreateDto(patientId, doctorId);
        var attendance = createDto.Adapt<Attendance>();

        context.Attendances.Add(attendance);
        await context.SaveChangesAsync();
        
        return attendance.Adapt<AttendanceDto>();
    }
}