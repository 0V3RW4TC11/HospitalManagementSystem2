using Domain.Entities;

namespace Abstractions
{
    public interface IUnitOfWork
    {
        // SEE DOMAIN / ENTITIES
        //IRepository<Account> Accounts { get; } 

        IRepository<Admin> Admins { get; }

        IRepository<Attendance> Attendances { get; }

        IRepository<Doctor> Doctors { get; }

        IRepository<DoctorSpecialization> DoctorSpecializations { get; }

        IIdentityProvider IdentityProvider { get; }

        IRepository<Patient> Patients { get; }

        IRepository<Specialization> Specializations { get; }

        Task RunInTransactionAsync(Func<CancellationToken, Task> operation, CancellationToken ct = default);

        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
