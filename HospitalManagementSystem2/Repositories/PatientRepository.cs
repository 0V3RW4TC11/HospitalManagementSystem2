using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem2.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly DbSet<Patient> _patients;

    public PatientRepository(IDbContext context)
    {
        _patients = context.Patients;
    }
    
    public IQueryable<Patient> Patients => _patients.AsNoTracking();
    
    public async Task AddAsync(Patient patient)
    {
        await _patients.AddAsync(patient);
    }

    public async Task UpdateAsync(Patient patient)
    {
        var entry = await _patients.SingleAsync(p => p.Id == patient.Id);
        
        entry.Title = patient.Title;
        entry.FirstName = patient.FirstName;
        entry.LastName = patient.LastName;
        entry.Email = patient.Email;
        entry.Phone = patient.Phone;
        entry.Address = patient.Address;
        entry.DateOfBirth = patient.DateOfBirth;
        entry.Gender = patient.Gender;
        entry.BloodType = patient.BloodType;
        
        _patients.Update(entry);
    }

    public async Task RemoveAsync(Patient patient)
    {
        var entry = await _patients.SingleAsync(p => p.Id == patient.Id);
        _patients.Remove(entry);
    }
}