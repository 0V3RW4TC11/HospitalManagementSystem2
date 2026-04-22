using Entities;

namespace Abstractions
{
    public interface IUnitOfWork
    {
        //IRepository<Account> Accounts { get; }    See Domain/Entities/Account.cs

        IRepository<Admin> Admins { get; }

        IRepository<Attendance> Attendances { get; }

        IRepository<Doctor> Doctors { get; }

        IRepository<DoctorSpecialization> DoctorSpecializations { get; }

        IIdentityService IdentityService { get; }

        IRepository<Patient> Patients { get; }

        IRepository<Specialization> Specializations { get; }

        Task RunInTransactionAsync(Func<CancellationToken, Task> operation, CancellationToken ct = default);

        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
