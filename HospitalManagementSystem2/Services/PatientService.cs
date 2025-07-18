using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Helpers;
using HospitalManagementSystem2.Models.Entities;
using HospitalManagementSystem2.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem2.Services;

public class PatientService
{
    private readonly ApplicationDbContext _context;
    private readonly AccountService _accountService;
    private readonly IPatientRepository _patientRepository;

    public PatientService(ApplicationDbContext context, 
                          AccountService accountService,
                          IPatientRepository patientRepository)
    {
        _context = context;
        _accountService = accountService;
        _patientRepository = patientRepository;
    }

    public async Task CreateAsync(Patient patient, string password)
    {
        // Check for existing Patient
        if (await IsExisting(patient)) throw new Exception("A duplicate record exists");
        
        // Validate password
        ArgumentException.ThrowIfNullOrWhiteSpace(password, nameof(password));
        
        // Create Patient and Account
        await TransactionHelper.ExecuteInTransactionAsync(_context, async () =>
        {
            // Create Patient
            await _patientRepository.AddAsync(patient);
            await _context.SaveChangesAsync();
            
            // Create Account
            await _accountService.CreateAsync(patient.Id, 
                Constants.AuthRoles.Patient, patient.Email, password);
        });
    }

    public async Task<Patient?> FindByIdAsync(Guid id)
    {
        return await _patientRepository.Patients.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task UpdateAsync(Patient patient)
    {
        await _patientRepository.UpdateAsync(patient);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Patient patient)
    {
        await TransactionHelper.ExecuteInTransactionAsync(_context, async () =>
        {
            // Delete the Patient
            await _patientRepository.RemoveAsync(patient);

            // Save changes to DbContext
            await _context.SaveChangesAsync();

            // Delete the Account
            await _accountService.DeleteByUserIdAsync(patient.Id);
        });
    }
    
    private async Task<bool> IsExisting(Patient patient)
        => await _patientRepository.Patients.AnyAsync(Patient.Matches(patient));
}