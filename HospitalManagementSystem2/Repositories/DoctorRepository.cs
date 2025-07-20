using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem2.Repositories;

public class DoctorRepository : IDoctorRepository
{
    private readonly DbSet<Doctor> _doctors;
    private readonly IDoctorSpecializationRepository _doctorSpecializationRepository;
    private readonly ISpecializationRepository _specializationRepository;
    
    public DoctorRepository(IDbContext context,
                            IDoctorSpecializationRepository doctorSpecializationRepository,
                            ISpecializationRepository specializationRepository)
    {
        _doctors = context.Doctors;
        _doctorSpecializationRepository = doctorSpecializationRepository;
        _specializationRepository = specializationRepository;
    }

    public IQueryable<Doctor> Doctors => _doctors.AsNoTracking();

    public async Task AddAsync(Doctor doctor)
    {
        await _doctors.AddAsync(doctor);
        await _doctorSpecializationRepository.AddRangeAsync(doctor.Specializations.Select(spec =>
            new DoctorSpecialization { DoctorId = doctor.Id, SpecializationId = spec.Id }));
    }

    public async Task<Doctor?> FindByIdAsync(Guid id)
    {
        var doctor = await Doctors.SingleOrDefaultAsync(d => d.Id == id);
        
        if (doctor == null) return null;
        
        var doctorSpecs 
            = await _doctorSpecializationRepository.DoctorSpecializations.Where(ds 
                => ds.DoctorId == id).ToArrayAsync();
        
        var specs = new List<Specialization>();
        foreach (var ds in doctorSpecs)
        {
            var spec = await _specializationRepository.Specializations
                .SingleAsync(s => s.Id == ds.SpecializationId);
            specs.Add(spec);
        }
        doctor.Specializations = specs;

        return doctor;
    }

    public async Task UpdateAsync(Doctor doctor)
    {
        var entry = await _doctors.SingleAsync(x => x.Id == doctor.Id);
        
        entry.FirstName = doctor.FirstName;
        entry.LastName = doctor.LastName;
        entry.Email = doctor.Email;
        entry.Phone = doctor.Phone;
        entry.Address = doctor.Address;
        entry.DateOfBirth = doctor.DateOfBirth;
        entry.Gender = doctor.Gender;
        
        _doctors.Update(entry);
        
        var existingSpecIds = await _doctorSpecializationRepository.DoctorSpecializations
            .Where(dspec => dspec.DoctorId == doctor.Id)
            .Select(dspec => dspec.SpecializationId)
            .ToArrayAsync();
        
        var newSpecIds = doctor.Specializations.Select(spec => spec.Id).ToArray();
        var intersect = newSpecIds.Intersect(existingSpecIds).ToArray();
        var idsToAdd = newSpecIds.Except(intersect).Select(id => new DoctorSpecialization
            { DoctorId = doctor.Id, SpecializationId = id });
        var idsToRemove = existingSpecIds.Except(intersect).Select(id => new DoctorSpecialization
            { DoctorId = doctor.Id, SpecializationId = id });
        
        await _doctorSpecializationRepository.AddRangeAsync(idsToAdd);
        await _doctorSpecializationRepository.RemoveRangeAsync(idsToRemove);
    }

    public async Task RemoveAsync(Doctor doctor)
    {
        var entry = await _doctors.SingleAsync(x => x.Id == doctor.Id);
        
        _doctors.Remove(entry);
    }
}