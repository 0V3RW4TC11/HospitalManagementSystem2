namespace Services.Abstractions;

public interface IHmsServiceProvider
{
    IAdminService AdminService { get; }
    IAttendanceService AttendanceService { get; }
    IDoctorService DoctorService { get; }
    IIdentityService IdentityService { get; }
    IPatientService PatientService { get; }
    ISpecializationService SpecializationService { get; }
}