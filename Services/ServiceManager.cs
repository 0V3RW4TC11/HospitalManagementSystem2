using Domain;
using Domain.Repositories;
using Services.Abstractions;

namespace Services;

public sealed class ServiceManager : IServiceManager
{
    public ServiceManager(IRepositoryManager repositoryManager)
    {
        var lazyAccountService = new Lazy<IAccountService>(() => new AccountService(repositoryManager));
        
        var lazyStaffEmailService = new Lazy<IStaffEmailService>(() => new StaffEmailService(repositoryManager.IdentityProvider));
        
        var lazyAdminService = new Lazy<IAdminService>(() => new AdminService(
            repositoryManager, 
            lazyAccountService.Value, 
            lazyStaffEmailService.Value));
        
        var lazyDoctorService = new Lazy<IDoctorService>(() => new DoctorService(
            repositoryManager,
            lazyAccountService.Value,
            lazyStaffEmailService.Value));
        
        var lazyPatientService = new Lazy<IPatientService>(() => new PatientService(
            repositoryManager,
            lazyAccountService.Value));
        
        var lazySpecializationService = new Lazy<ISpecializationService>(() => new SpecializationService(repositoryManager));
        
        var lazyAttendanceService = new Lazy<IAttendanceService>(() => new AttendanceService(repositoryManager));
        
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