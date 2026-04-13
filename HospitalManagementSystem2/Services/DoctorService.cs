using System.Collections.Immutable;
using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Helpers;
using HospitalManagementSystem2.Models.Entities;
using HospitalManagementSystem2.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace HospitalManagementSystem2.Services;

public class DoctorService
{
    private readonly ApplicationDbContext _context;
    private readonly AccountService _accountService;
    private readonly DoctorSpecializationService _doctorSpecializationService;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IStaffEmailGenerator _staffEmailGenerator;

    public DoctorService(ApplicationDbContext context,
        AccountService accountService,
        DoctorSpecializationService doctorSpecializationService,
        IDoctorRepository doctorRepository,
        IStaffEmailGenerator staffEmailGenerator)
    {
        _context = context;
        _accountService = accountService;
        _doctorSpecializationService = doctorSpecializationService;
        _doctorRepository = doctorRepository;
        _staffEmailGenerator = staffEmailGenerator;
    }

    public async Task CreateAsync(Doctor doctor, string password)
    {
        // Check for existing Doctor
        if (await IsExistingAsync(doctor))
            throw new Exception("A duplicate record exists");
        
        // Validate Doctor details
        ValidateDoctorDetails(doctor);
        
        // Validate Specialization details
        ValidateSpecializationDetails(doctor.Specializations);

        await TransactionHelper.ExecuteInTransactionAsync(_context, async () =>
        {
            // Create Doctor
            await _doctorRepository.AddAsync(doctor);
            
            // Create username
            var username = await _staffEmailGenerator
                .GenerateEmailAsync(doctor.FirstName, doctor.LastName, Constants.StaffEmailDomain);

            // Create Account
            await _accountService.CreateAsync(doctor.Id, Constants.AuthRoles.Doctor, username, password);

            // Save DbContext changes
            await _context.SaveChangesAsync();
        });
    }

    public async Task<Doctor?> FindByIdAsync(Guid id)
    {
        // Get Doctor
        var doctor = await _doctorRepository.Doctors.FirstOrDefaultAsync(x => x.Id == id);
        if (doctor == null) return null;
        
        // Get Doctor Specializations
        var specializations
            = await _doctorSpecializationService.GetDoctorSpecializationsAsync(doctor.Id);
        
        // Specializations null check
        if (specializations == null)
            throw new Exception("Doctor missing Specializations");
        
        return doctor;
    }

    public async Task UpdateAsync(Doctor doctor)
    {
        if (doctor.Id == Guid.Empty)
            throw new Exception("Doctor Id cannot be empty");
        
        // Validate Doctor details
        ValidateDoctorDetails(doctor);
        // Validate Specialization details
        ValidateSpecializationDetails(doctor.Specializations);
        
        await TransactionHelper.ExecuteInTransactionAsync(_context, async () =>
        {
            // Update Doctor
            await _doctorRepository.UpdateAsync(doctor);

            // Update DoctorSpecializations
            await _doctorSpecializationService.UpdateDoctorSpecializationsAsync(doctor);

            // Save DbContext changes
            await _context.SaveChangesAsync();
        });
    }
    
    public async Task DeleteAsync(Doctor doctor)
    {
        if (doctor.Id == Guid.Empty)
            throw new Exception("Doctor Id cannot be empty");
        
        await TransactionHelper.ExecuteInTransactionAsync(_context, async () =>
        {
            // Delete Doctor
            await _doctorRepository.RemoveAsync(doctor);
            
            // Save DbContext changes
            await _context.SaveChangesAsync();

            // Delete Account
            await _accountService.DeleteByUserIdAsync(doctor.Id);
        });
    }

    private async Task<bool> IsExistingAsync(Doctor doctor)
        => await _doctorRepository.Doctors.AnyAsync(Doctor.Matches(doctor));
    
    // simple checks for now, but can extend to more comprehensive checks later
    // would likely include validation for properties in the entity class itself
    private static void ValidateDoctorDetails(Doctor doctor)
    {
        ArgumentNullException.ThrowIfNull(doctor.DateOfBirth, nameof(doctor.DateOfBirth));
        ArgumentException.ThrowIfNullOrWhiteSpace(doctor.FirstName, nameof(doctor.FirstName));
        ArgumentException.ThrowIfNullOrWhiteSpace(doctor.LastName, nameof(doctor.LastName));
        ArgumentException.ThrowIfNullOrWhiteSpace(doctor.Email, nameof(doctor.Email));
        ArgumentException.ThrowIfNullOrWhiteSpace(doctor.Phone, nameof(doctor.Phone));
        ArgumentException.ThrowIfNullOrWhiteSpace(doctor.Address, nameof(doctor.Address));
    }
    
    private static void ValidateSpecializationDetails(IEnumerable<Specialization> specializations)
    {
        ArgumentNullException.ThrowIfNull(specializations, nameof(specializations));
        var materialized = specializations.ToArray();
        
        if(materialized.Length == 0)
            throw new Exception("Specializations cannot be empty");
        if(materialized.Any(s => s.Id == Guid.Empty))
            throw new Exception("Specializations cannot contain empty Ids");
    }
}