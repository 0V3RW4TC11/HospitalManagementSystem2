using Abstractions;
using Entities;
using Microsoft.AspNetCore.Identity;

namespace Persistence
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly HmsDbContext _context;
        private readonly Lazy<IRepository<Admin>> _lazyAdmins;
        private readonly Lazy<IRepository<Attendance>> _lazyAttendances;
        private readonly Lazy<IRepository<Doctor>> _lazyDoctors;
        private readonly Lazy<IRepository<DoctorSpecialization>> _lazyDoctorSpecializations;
        private readonly Lazy<IIdentityService> _lazyIdentityService;
        private readonly Lazy<IRepository<Patient>> _lazyPatients;
        private readonly Lazy<IRepository<Specialization>> _lazySpecializations;

        public UnitOfWork(HmsDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _lazyAdmins = new Lazy<IRepository<Admin>>(() => new Repository<Admin>(_context));
            _lazyAttendances = new Lazy<IRepository<Attendance>>(() => new Repository<Attendance>(_context));
            _lazyDoctors = new Lazy<IRepository<Doctor>>(() => new Repository<Doctor>(_context));
            _lazyDoctorSpecializations = new Lazy<IRepository<DoctorSpecialization>>(() => new Repository<DoctorSpecialization>(_context));
            _lazyIdentityService = new Lazy<IIdentityService>(() => new IdentityService(userManager));
            _lazyPatients = new Lazy<IRepository<Patient>>(() => new Repository<Patient>(_context));
            _lazySpecializations = new Lazy<IRepository<Specialization>>(() => new Repository<Specialization>(_context));
        }

        public IRepository<Admin> Admins => _lazyAdmins.Value;

        public IRepository<Attendance> Attendances => _lazyAttendances.Value;

        public IRepository<Doctor> Doctors => _lazyDoctors.Value;

        public IRepository<DoctorSpecialization> DoctorSpecializations => _lazyDoctorSpecializations.Value;

        public IIdentityService IdentityService => _lazyIdentityService.Value;

        public IRepository<Patient> Patients => _lazyPatients.Value;

        public IRepository<Specialization> Specializations => _lazySpecializations.Value;

        public async Task RunInTransactionAsync(Func<CancellationToken, Task> operation, CancellationToken ct = default)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(ct);

            try
            {
                await operation(ct);
                await transaction.CommitAsync(ct);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(ct);
                _context.ChangeTracker.Clear();
                throw;
            }
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}
