using Domain.Providers;
using Domain.Repositories;

namespace Domain;

public interface IUnitOfWork
{
    IAccountRepository AccountRepository { get; }
    
    IAdminRepository AdminRepository { get; }
    
    IAttendanceRepository AttendanceRepository { get; }
    
    IDoctorRepository DoctorRepository { get; }
    
    IDoctorSpecializationRepository DoctorSpecializationRepository { get; }
    
    IPatientRepository PatientRepository { get; }
    
    ISpecializationRepository SpecializationRepository { get; }
    
    IIdentityProvider IdentityProvider { get; }
    
    Task BeginTransactionAsync();
    
    Task CommitTransactionAsync();
    
    Task RollbackTransactionAsync();
    
    Task SaveChangesAsync();
}