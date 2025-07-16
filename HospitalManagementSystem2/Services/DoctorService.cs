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
    private readonly IDoctorRepository _doctorRepository;
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IStaffEmailGenerator _staffEmailGenerator;

    public DoctorService(ApplicationDbContext context,
        AccountService accountService,
        IDoctorRepository doctorRepository,
        ISpecializationRepository specializationRepository,
        IStaffEmailGenerator staffEmailGenerator)
    {
        _context = context;
        _accountService = accountService;
        _doctorRepository = doctorRepository;
        _specializationRepository = specializationRepository;
        _staffEmailGenerator = staffEmailGenerator;
    }

    public async Task CreateAsync(Doctor doctor, string password)
    {
        // Check for existing Doctor
        if (await IsExistingAsync(doctor))
            throw new Exception("A duplicate record exists");
        
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
        if (doctor == null)
            throw new Exception($"Doctor with Id {id} not found");
        if (doctor.Specializations.IsNullOrEmpty())
            throw new Exception($"Doctor with Id {id} has no specializations");
        
        return doctor;
    }

    public async Task UpdateAsync(Doctor doctor)
    {
        if (doctor.Id == Guid.Empty)
            throw new Exception("Doctor Id cannot be empty");
        
        // Validate Specialization details
        ValidateSpecializationDetails(doctor.Specializations);
        
        await TransactionHelper.ExecuteInTransactionAsync(_context, async () =>
        {
            // Update Doctor
            await _doctorRepository.UpdateAsync(doctor);

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
    
    private static void ValidateSpecializationDetails(IEnumerable<Specialization> specializations)
    {
        if (specializations.IsNullOrEmpty())
            throw new Exception("Specializations cannot be null or empty");
    }
}