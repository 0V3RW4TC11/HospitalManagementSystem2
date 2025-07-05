using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Models.Entities;
using HospitalManagementSystem2.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace HospitalManagementSystem2.Services
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<Doctor> _doctors;
        private readonly DbSet<DoctorSpecialization> _doctorspecs;
        private readonly DbSet<Specialization> _specializations;
        private readonly AccountHelper _accountHelper;
        private readonly IStaffEmailGenerator _staffEmailGenerator;

        public DoctorRepository(ApplicationDbContext context, 
                                AccountHelper accountHelper,
                                IStaffEmailGenerator staffEmailGenerator)
        {
            _context = context;
            _doctors = context.Doctors;
            _doctorspecs = context.DoctorSpecializations;
            _specializations = context.Specializations;
            _accountHelper = accountHelper;
            _staffEmailGenerator = staffEmailGenerator;
        }

        public IQueryable<Doctor> Doctors => _doctors.AsNoTracking();

        public async Task CreateAsync(Doctor doctor, string password)
        {
            if (await IsExisting(doctor))
                throw new Exception("A duplicate record exists");

            await TransactionHelper.ExecuteInTransaction(_context, async () => 
            {
                // Create the Username
                var username = await _staffEmailGenerator.GenerateEmailAsync(doctor, Constants.StaffEmailDomain);
                // Create the Account
                await _accountHelper.CreateAsync(doctor, Constants.AuthRoles.Doctor, username, password);

                // Create the Doctor
                await _doctors.AddAsync(doctor);
                await _context.SaveChangesAsync();

                // Check that a specialization has been added
                if (doctor.Specializations.IsNullOrEmpty())
                    throw new Exception("Doctor needs at least one Specialization");

                // Create DoctorSpecializations
                foreach (var spec in doctor.Specializations)
                {
                    await _doctorspecs.AddAsync(new DoctorSpecialization { DoctorId = doctor.Id, SpecializationId = spec.Id });
                }
                await _context.SaveChangesAsync();
            });
        }

        public async Task DeleteAsync(Doctor doctor)
        {
            await TransactionHelper.ExecuteInTransaction(_context, async () =>
            {
                // Delete Doctor
                _doctors.Remove(doctor);
                await _context.SaveChangesAsync();

                // Delete Doctor's Account
                await _accountHelper.DeleteAsync(doctor);
            });
        }

        public async Task<Doctor?> FindByIdAsync(Guid id)
        {
            // Retrieve Doctor
            var doctor = await Doctors.FirstOrDefaultAsync(x => x.Id == id);
            
            // Check if Doctor is null
            if (doctor == null) 
                return null;

            // Get SpecIds for Doctor
            var specIds = await _doctorspecs
                .Where(dspec => dspec.DoctorId == doctor.Id)
                .Select(dspec => dspec.SpecializationId)
                .ToArrayAsync();

            // Get Specializations for Doctor
            var specs = await _specializations
                .Where(spec => specIds.Any(sid => sid == spec.Id))
                .ToArrayAsync();

            // Set Doctor Specializations
            doctor.Specializations = specs;

            return doctor;
        }

        public async Task UpdateAsync(Doctor doctor)
        {
            await TransactionHelper.ExecuteInTransaction(_context, async () =>
            {
                // Update Doctor
                var entry = _context.Entry(doctor);
                entry.State = EntityState.Modified;

                // Update DoctorSpecializations
                var existingSpecIds = await _doctorspecs
                    .Where(dspec => dspec.DoctorId == doctor.Id)
                    .Select(dspec => dspec.SpecializationId)
                    .ToArrayAsync();
                var newSpecIds = doctor.Specializations.Select(spec => spec.Id);
                var intersect = newSpecIds.Intersect(existingSpecIds);
                var idsToAdd = newSpecIds.Except(intersect).Select(id => new DoctorSpecialization { DoctorId = doctor.Id, SpecializationId = id });
                var idsToRemove = existingSpecIds.Except(intersect).Select(id => new DoctorSpecialization { DoctorId = doctor.Id, SpecializationId = id });

                await _doctorspecs.AddRangeAsync(idsToAdd);
                _doctorspecs.RemoveRange(idsToRemove);

                // Save changes
                await _context.SaveChangesAsync();

                // Update Doctor Account
                await _accountHelper.UpdateAsync(doctor);
            });
        }

        private async Task<bool> IsExisting(Doctor doctor) => await _doctors.AnyAsync(Staff.Matches(doctor));
    }
}
