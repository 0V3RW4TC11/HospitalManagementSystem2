namespace Services.Abstractions;

public interface IServiceManager
{
    IAccountService AccountService { get; }

    IAdminService AdminService { get; }
    
    IPatientService PatientService { get; }
    
    IDoctorService DoctorService { get; }
    
    ISpecializationService SpecializationService { get; }
    
    IAttendanceService AttendanceService { get; }
}