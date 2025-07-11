using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Helpers;
using HospitalManagementSystem2.Models.Entities;
using HospitalManagementSystem2.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem2.Services;

public class AdminService
{
    private readonly ApplicationDbContext _context;
    private readonly AccountService _accountService;
    private readonly IAdminRepository _adminRepository;
    private readonly IStaffEmailGenerator _staffEmailGenerator;

    public AdminService(ApplicationDbContext context,
        AccountService accountService,
        IAdminRepository adminRepository,
        IStaffEmailGenerator staffEmailGenerator)
    {
        _context = context;
        _accountService = accountService;
        _adminRepository = adminRepository;
        _staffEmailGenerator = staffEmailGenerator;
    }

    public async Task CreateAsync(Admin admin, string password)
    {
        if (await IsExisting(admin)) throw new Exception("A duplicate record exists");
        
        // Validate Admin details
        ValidateAdminDetailsThrowsException(admin);

        await TransactionHelper.ExecuteInTransactionAsync(_context, async () =>
        {
            // Create Admin
            await _adminRepository.AddAsync(admin);
            
            // Save changes to DbContext
            await _context.SaveChangesAsync();
            
            // Create Username
            var username = await _staffEmailGenerator
                .GenerateEmailAsync(admin.FirstName, admin.LastName, Constants.StaffEmailDomain);
            
            // Create Account
            await _accountService.CreateAsync(admin.Id, Constants.AuthRoles.Admin, username, password);
        });
    }

    public async Task<Admin?> FindByIdAsync(Guid id)
    {
        return await _adminRepository.Admins.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task UpdateAsync(Admin admin)
    {
        if (admin.Id == Guid.Empty)
            throw new Exception("Admin Id cannot be empty");
        
        // Validate Admin details
        ValidateAdminDetailsThrowsException(admin);
        
        // Update Admin
        await _adminRepository.UpdateAsync(admin);
            
        // Save changes to DbContext
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Admin admin)
    {
        await TransactionHelper.ExecuteInTransactionAsync(_context, async () =>
        {
            // Delete the Admin
            await _adminRepository.RemoveAsync(admin);
            
            // Save changes to DbContext
            await _context.SaveChangesAsync();
            
            // Delete the Account
            await _accountService.DeleteByUserIdAsync(admin.Id);
        });
    }

    private async Task<bool> IsExisting(Admin admin)
        => await _adminRepository.Admins.AnyAsync(Admin.Matches(admin));

    private static void ValidateAdminDetailsThrowsException(Admin admin)
    {
        ArgumentNullException.ThrowIfNull(admin.DateOfBirth, nameof(admin.DateOfBirth));
        ArgumentException.ThrowIfNullOrWhiteSpace(admin.FirstName, nameof(admin.FirstName));
        ArgumentException.ThrowIfNullOrWhiteSpace(admin.Email, nameof(admin.Email));
        ArgumentException.ThrowIfNullOrWhiteSpace(admin.Phone, nameof(admin.Phone));
    }
}