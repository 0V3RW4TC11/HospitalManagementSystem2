namespace Domain.Repositories;

public interface IRepositoryManager
{
    IAccountRepository AccountRepository { get; }
    
    IAdminRepository AdminRepository { get; }
    
    IAttendanceRepository AttendanceRepository { get; }
    
    IDoctorRepository DoctorRepository { get; }
    
    IDoctorSpecializationRepository DoctorSpecializationRepository { get; }
    
    IPatientRepository PatientRepository { get; }
    
    ISpecializationRepository SpecializationRepository { get; }
    
    IIdentityProvider IdentityProvider { get; }
    
    IUnitOfWork UnitOfWork { get; }
}