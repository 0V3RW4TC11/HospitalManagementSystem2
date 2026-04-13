using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Helpers;
using HospitalManagementSystem2.Models.Entities;
using HospitalManagementSystem2.Repositories;

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
        // Validate password
        ArgumentException.ThrowIfNullOrWhiteSpace(password, nameof(password));
        
        // Validate Patient
        ValidatePatientDetailsThrowsException(patient);
        
        // Create Patient and Account
        await TransactionHelper.ExecuteInTransactionAsync(_context, async () =>
        {
            // Create Patient
            await _patientRepository.AddAsync(patient);
            
            // Save changes to DbContext
            await _context.SaveChangesAsync();
            
            // Create Account
            await _accountService.CreateAsync(patient.Id, 
                                          Constants.AuthRoles.Patient,
                                              patient.Email, 
                                              password);
        });
    }

    public async Task<Patient?> FindByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(Patient patient)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(Patient patient)
    {
        throw new NotImplementedException();   
    }

    private static void ValidatePatientDetailsThrowsException(Patient patient)
    {
        ArgumentNullException.ThrowIfNull(patient.DateOfBirth, nameof(patient.DateOfBirth));
        ArgumentNullException.ThrowIfNull(patient.BloodType, nameof(patient.BloodType));
        ArgumentException.ThrowIfNullOrWhiteSpace(patient.FirstName, nameof(patient.FirstName));
        ArgumentException.ThrowIfNullOrWhiteSpace(patient.Email, nameof(patient.Email));
        ArgumentException.ThrowIfNullOrWhiteSpace(patient.Gender, nameof(patient.Gender));
    }
}