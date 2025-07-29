using Domain.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Persistence.Repositories;

public sealed class RepositoryManager : IRepositoryManager
{
    public RepositoryManager(RepositoryDbContext context, UserManager<IdentityUser> userManager)
    {
        var lazyAccountRepository = new Lazy<IAccountRepository>(() => new AccountRepository(context));
        var lazyAdminRepository = new Lazy<IAdminRepository>(() => new AdminRepository(context));
        var lazyAttendanceRepository = new Lazy<IAttendanceRepository>(() => new AttendanceRepository(context));
        var lazyDoctorRepository = new Lazy<IDoctorRepository>(() => new DoctorRepository(context));
        var lazyDoctorSpecializationRepository = new Lazy<IDoctorSpecializationRepository>(() => new DoctorSpecializationRepository(context));
        var lazyPatientRepository = new Lazy<IPatientRepository>(() => new PatientRepository(context));
        var lazySpecializationRepository = new Lazy<ISpecializationRepository>(() => new SpecializationRepository(context));
        var lazyIdentityProvider = new Lazy<IIdentityProvider>(() => new IdentityProvider(userManager));
        var lazyUnitOfWork = new Lazy<IUnitOfWork>(() => new UnitOfWork(context));
        
        AccountRepository = lazyAccountRepository.Value;
        AdminRepository = lazyAdminRepository.Value;
        AttendanceRepository = lazyAttendanceRepository.Value;
        DoctorRepository = lazyDoctorRepository.Value;
        DoctorSpecializationRepository = lazyDoctorSpecializationRepository.Value;
        PatientRepository = lazyPatientRepository.Value;
        SpecializationRepository = lazySpecializationRepository.Value;
        IdentityProvider = lazyIdentityProvider.Value;
        UnitOfWork = lazyUnitOfWork.Value;
    }
    
    public IAccountRepository AccountRepository { get; }
    public IAdminRepository AdminRepository { get; }
    public IAttendanceRepository AttendanceRepository { get; }
    public IDoctorRepository DoctorRepository { get; }
    public IDoctorSpecializationRepository DoctorSpecializationRepository { get; }
    public IPatientRepository PatientRepository { get; }
    public ISpecializationRepository SpecializationRepository { get; }
    public IIdentityProvider IdentityProvider { get; }
    public IUnitOfWork UnitOfWork { get; }
}