using Domain.Repositories;
using Services.Abstractions;
using Services.Helpers;

namespace Services;

public sealed class ServiceManager : IServiceManager
{
    public ServiceManager(IRepositoryManager repositoryManager)
    {
        var lazyAccountHelper = new Lazy<AccountHelper>(() => new AccountHelper(repositoryManager));

        var lazyStaffEmailService = new Lazy<IStaffEmailService>(() => new StaffEmailService(repositoryManager.IdentityProvider));

        var lazyIdentityService = new Lazy<IIdentityService>(() => new IdentityService(repositoryManager));

        var lazyAdminService = new Lazy<IAdminService>(() => new AdminService(
            repositoryManager,
            lazyStaffEmailService.Value,
            lazyAccountHelper.Value));

        var lazyDoctorService = new Lazy<IDoctorService>(() => new DoctorService(
            repositoryManager,
            lazyStaffEmailService.Value,
            lazyAccountHelper.Value));

        var lazyPatientService = new Lazy<IPatientService>(() => new PatientService(
            repositoryManager,
            lazyAccountHelper.Value));

        var lazySpecializationService = new Lazy<ISpecializationService>(() => new SpecializationService(repositoryManager));

        var lazyAttendanceService = new Lazy<IAttendanceService>(() => new AttendanceService(repositoryManager));

        var lazyAccountService = new Lazy<IAccountService>(() => new AccountService(repositoryManager.AccountRepository));

        IdentityService = lazyIdentityService.Value;
        AdminService = lazyAdminService.Value;
        PatientService = lazyPatientService.Value;
        DoctorService = lazyDoctorService.Value;
        SpecializationService = lazySpecializationService.Value;
        AttendanceService = lazyAttendanceService.Value;
        AccountService = lazyAccountService.Value;
    }

    public IIdentityService IdentityService { get; }
    public IAdminService AdminService { get; }
    public IPatientService PatientService { get; }
    public IDoctorService DoctorService { get; }
    public ISpecializationService SpecializationService { get; }
    public IAttendanceService AttendanceService { get; }
    public IAccountService AccountService { get; }
}