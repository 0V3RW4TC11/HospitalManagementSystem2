using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Models.Entities;
using HospitalManagementSystem2.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace HospitalManagementSystem2.Services;

public class DoctorSpecializationService
{
    private readonly ApplicationDbContext _context;
    private readonly IDoctorSpecializationRepository _doctorSpecializationRepository;
    private readonly ISpecializationRepository _specializationRepository;

    public DoctorSpecializationService(ApplicationDbContext context,
        IDoctorSpecializationRepository doctorSpecializationRepository,
        ISpecializationRepository specializationRepository)
    {
        _context = context;
        _doctorSpecializationRepository = doctorSpecializationRepository;
        _specializationRepository = specializationRepository;
    }

    public async Task CreateDoctorSpecializationsAsync(Guid doctorId, IEnumerable<Specialization> specializations)
    {
        // Create Id list to populate
        var docSpecList = new List<DoctorSpecialization>();
        
        // Verify specializations exist and add them to list
        foreach (var spec in specializations)
        {
            if (!await IsExisting(spec))
                throw new Exception("Specialization not found in database");
            
            docSpecList.Add(new DoctorSpecialization { DoctorId = doctorId, SpecializationId = spec.Id });
        }
        
        // Add DoctorSpecializations
        await _doctorSpecializationRepository.AddRangeAsync(docSpecList);
        
        // Save DbContext
        await _context.SaveChangesAsync();
    }
    
    public async Task<IEnumerable<Specialization>?> GetDoctorSpecializationsAsync(Guid doctorId)
    {
        // Get Specialization Ids for Doctor
        var specIds = await _doctorSpecializationRepository
            .DoctorSpecializations
            .Where(dspec => dspec.DoctorId == doctorId)
            .Select(dspec => dspec.SpecializationId)
            .ToArrayAsync();
        
        // Specialization Ids null check
        if (specIds.IsNullOrEmpty()) return null;
        
        // Create Specialization List to populate with valid Specializations
        var specList = new List<Specialization>();
        
        // Populate List with existing Specializations
        foreach (var specId in specIds)
        {
            var spec = await _specializationRepository
                           .Specializations
                           .FirstOrDefaultAsync(x => x.Id == specId)
                       ?? throw new Exception($"Specialization with Id {specId} not found");
            specList.Add(spec);
        }
        
        return specList;
    }

    public async Task UpdateDoctorSpecializationsAsync(Doctor doctor)
    {
        // Update DoctorSpecializations
        var existingSpecIds = await _doctorSpecializationRepository.DoctorSpecializations
            .Where(dspec => dspec.DoctorId == doctor.Id)
            .Select(dspec => dspec.SpecializationId)
            .ToArrayAsync();
        var newSpecIds = doctor.Specializations.Select(spec => spec.Id);
        var intersect = newSpecIds.Intersect(existingSpecIds);
        var idsToAdd = newSpecIds.Except(intersect).Select(id => new DoctorSpecialization
            { DoctorId = doctor.Id, SpecializationId = id });
        var idsToRemove = existingSpecIds.Except(intersect).Select(id => new DoctorSpecialization
            { DoctorId = doctor.Id, SpecializationId = id });
        
        // Add and Remove DoctorSpecializations
        await _doctorSpecializationRepository.AddRangeAsync(idsToAdd);
        await _doctorSpecializationRepository.RemoveRangeAsync(idsToRemove);
        
        // Save DbContext
        await _context.SaveChangesAsync();
    }
    
    private async Task<bool> IsExisting(Specialization specialization)
        => await _specializationRepository.Specializations.AnyAsync(s => s.Id == specialization.Id);
}