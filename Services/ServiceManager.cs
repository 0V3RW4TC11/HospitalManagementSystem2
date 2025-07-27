using Domain;
using Services.Abstractions;

namespace Services;

public class ServiceManager : IServiceManager
{
    public ServiceManager(IUnitOfWork unitOfWork)
    {
        var lazyAccountService = new Lazy<IAccountService>(() => new AccountService(unitOfWork));
        
        var lazyStaffEmailService = new Lazy<IStaffEmailService>(() => new StaffEmailService(unitOfWork.IdentityProvider));
        
        var lazyAdminService = new Lazy<IAdminService>(() => new AdminService(
            unitOfWork, 
            lazyAccountService.Value, 
            lazyStaffEmailService.Value));
        
        var lazyDoctorService = new Lazy<IDoctorService>(() => new DoctorService(
            unitOfWork,
            lazyAccountService.Value,
            lazyStaffEmailService.Value));
        
        var lazyPatientService = new Lazy<IPatientService>(() => new PatientService(
            unitOfWork,
            lazyAccountService.Value));
        
        var lazySpecializationService = new Lazy<ISpecializationService>(() => new SpecializationService(unitOfWork));
        
        var lazyAttendanceService = new Lazy<IAttendanceService>(() => new AttendanceService(unitOfWork));
        
        AdminService = lazyAdminService.Value;
        PatientService = lazyPatientService.Value;
        DoctorService = lazyDoctorService.Value;
        SpecializationService = lazySpecializationService.Value;
        AttendanceService = lazyAttendanceService.Value;
    }
    
    public IAdminService AdminService { get; }
    public IPatientService PatientService { get; }
    public IDoctorService DoctorService { get; }
    public ISpecializationService SpecializationService { get; }
    public IAttendanceService AttendanceService { get; }
}