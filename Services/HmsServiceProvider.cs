using Domain;
using Services.Abstractions;

namespace Services;

public sealed class HmsServiceProvider : IHmsServiceProvider
{
    private readonly Lazy<AccountService> _lazyAccountService;
    private readonly Lazy<IAdminService> _lazyAdminService;
    private readonly Lazy<IAttendanceService> _lazyAttendanceService;
    private readonly Lazy<IDoctorService> _lazyDoctorService;
    private readonly Lazy<DoctorSpecializationService> _lazyDoctorSpecializationService;
    private readonly Lazy<IIdentityService> _lazyIdentityService;
    private readonly Lazy<IPatientService> _lazyPatientService;
    private readonly Lazy<ISpecializationService> _lazySpecializationService;
    private readonly Lazy<StaffEmailService> _lazyStaffEmailService;

    public HmsServiceProvider(IUnitOfWork unitOfWork)
    {
        _lazyAccountService = new Lazy<AccountService>(() => 
            new AccountService(unitOfWork));

        _lazyDoctorSpecializationService = new Lazy<DoctorSpecializationService>(() => 
            new DoctorSpecializationService(unitOfWork));

        _lazyStaffEmailService = new Lazy<StaffEmailService>(() => 
            new StaffEmailService(unitOfWork.IdentityProvider));

        _lazyIdentityService = new Lazy<IIdentityService>(() => 
            new IdentityService(unitOfWork.IdentityProvider, AccountService));

        _lazyAdminService = new Lazy<IAdminService>(() => new AdminService(
            AccountService,
            StaffEmailService,
            unitOfWork));

        _lazyDoctorService = new Lazy<IDoctorService>(() => new DoctorService(
            AccountService,
            DoctorSpecializationService,
            StaffEmailService,
            unitOfWork));

        _lazyPatientService = new Lazy<IPatientService>(() => new PatientService(
            AccountService,
            unitOfWork));

        _lazySpecializationService = new Lazy<ISpecializationService>(() => new SpecializationService(unitOfWork));

        _lazyAttendanceService = new Lazy<IAttendanceService>(() => new AttendanceService(unitOfWork));
    }

    public IAdminService AdminService => _lazyAdminService.Value;

    public IAttendanceService AttendanceService => _lazyAttendanceService.Value;

    public IDoctorService DoctorService => _lazyDoctorService.Value;

    public IIdentityService IdentityService => _lazyIdentityService.Value;

    public IPatientService PatientService => _lazyPatientService.Value;

    public ISpecializationService SpecializationService => _lazySpecializationService.Value;

    private AccountService AccountService => _lazyAccountService.Value;

    private DoctorSpecializationService DoctorSpecializationService => _lazyDoctorSpecializationService.Value;

    private StaffEmailService StaffEmailService => _lazyStaffEmailService.Value;
}