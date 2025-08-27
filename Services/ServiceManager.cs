using Domain.Repositories;
using Services.Abstractions;

namespace Services;

public sealed class ServiceManager : IServiceManager
{
    public ServiceManager(IRepositoryManager repositoryManager)
    {
        var lazyAccountManager = new Lazy<IAccountManager>(() => new AccountManager(repositoryManager));
        
        var lazyStaffEmailService = new Lazy<IStaffEmailService>(() => new StaffEmailService(repositoryManager.IdentityProvider));

        var lazyAccountService = new Lazy<IAccountService>(() => new AccountService(repositoryManager));
        
        var lazyAdminService = new Lazy<IAdminService>(() => new AdminService(
            repositoryManager, 
            lazyAccountManager.Value, 
            lazyStaffEmailService.Value));
        
        var lazyDoctorService = new Lazy<IDoctorService>(() => new DoctorService(
            repositoryManager,
            lazyAccountManager.Value,
            lazyStaffEmailService.Value));
        
        var lazyPatientService = new Lazy<IPatientService>(() => new PatientService(
            repositoryManager,
            lazyAccountManager.Value));
        
        var lazySpecializationService = new Lazy<ISpecializationService>(() => new SpecializationService(repositoryManager));
        
        var lazyAttendanceService = new Lazy<IAttendanceService>(() => new AttendanceService(repositoryManager));
        
        AccountService = lazyAccountService.Value;
        AdminService = lazyAdminService.Value;
        PatientService = lazyPatientService.Value;
        DoctorService = lazyDoctorService.Value;
        SpecializationService = lazySpecializationService.Value;
        AttendanceService = lazyAttendanceService.Value;
    }

    public IAccountService AccountService { get; }
    public IAdminService AdminService { get; }
    public IPatientService PatientService { get; }
    public IDoctorService DoctorService { get; }
    public ISpecializationService SpecializationService { get; }
    public IAttendanceService AttendanceService { get; }
}