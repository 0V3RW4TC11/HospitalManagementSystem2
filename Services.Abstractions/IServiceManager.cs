namespace Services.Abstractions;

public interface IServiceManager
{
    IIdentityService IdentityService { get; }

    IAdminService AdminService { get; }
    
    IPatientService PatientService { get; }
    
    IDoctorService DoctorService { get; }
    
    ISpecializationService SpecializationService { get; }
    
    IAttendanceService AttendanceService { get; }

    IAccountService AccountService { get; }
}