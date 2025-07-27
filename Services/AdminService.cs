using DataTransfer.Admin;
using Domain;
using Domain.Constants;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using Mapster;
using Services.Abstractions;
using Services.Helpers;

namespace Services;

internal sealed class AdminService : IAdminService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAccountService _accountService;
    private readonly IStaffEmailService _staffEmailService;

    public AdminService(
        IUnitOfWork unitOfWork, 
        IAccountService accountService,
        IStaffEmailService staffEmailService)
    {
        _unitOfWork = unitOfWork;
        _accountService = accountService;
        _staffEmailService = staffEmailService;
    }
    
    public async Task CreateAsync(AdminCreateDto adminCreateDto, CancellationToken cancellationToken = default)
    {
        ValidateAdminCreateDto(adminCreateDto);
        
        await TransactionHelper.ExecuteInTransactionAsync(_unitOfWork, async () =>
        {
            var admin = adminCreateDto.Adapt<Admin>();
            _unitOfWork.AdminRepository.Add(admin);
            await _unitOfWork.SaveChangesAsync();
            
            var username = await _staffEmailService.CreateStaffEmailAsync(adminCreateDto.FirstName, adminCreateDto.LastName);
            
            await _accountService.CreateAsync(admin.Id, AuthRoles.Admin, username, adminCreateDto.Password);
        });
    }

    public async Task<AdminDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var admin = await GetAdminByIdAsync(id);
        
        return admin.Adapt<AdminDto>();
    }

    public async Task UpdateAsync(AdminDto adminDto, CancellationToken cancellationToken = default)
    {
        var admin = await GetAdminByIdAsync(adminDto.Id);
        
        ValidateAdminBaseDto(adminDto);
        
        admin.FirstName = adminDto.FirstName;
        admin.LastName = adminDto.LastName;
        admin.Gender = adminDto.Gender;
        admin.Address = adminDto.Address;
        admin.Phone = adminDto.Phone;
        admin.Email = adminDto.Email;
        admin.DateOfBirth = adminDto.DateOfBirth;
        
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var admin = await GetAdminByIdAsync(id);
        
        await TransactionHelper.ExecuteInTransactionAsync(_unitOfWork, async () =>
        {
            _unitOfWork.AdminRepository.Remove(admin);
            await _unitOfWork.SaveChangesAsync();
            await _accountService.DeleteByUserIdAsync(admin.Id);
        });
    }

    private async Task<Admin> GetAdminByIdAsync(Guid id)
    {
        var admin = await _unitOfWork.AdminRepository.FindByIdAsync(id);
        if (admin is null)
            throw new AdminNotFoundException(id.ToString());
        
        return admin;
    }

    private static void ValidateAdminBaseDto(AdminBaseDto baseDto)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(baseDto.FirstName, nameof(baseDto.FirstName));
        ArgumentException.ThrowIfNullOrWhiteSpace(baseDto.Phone, nameof(baseDto.Phone));
        ArgumentException.ThrowIfNullOrWhiteSpace(baseDto.Email, nameof(baseDto.Email));
    }
    
    private static void ValidateAdminCreateDto(AdminCreateDto adminCreateDto)
    {
        ValidateAdminBaseDto(adminCreateDto);
        ArgumentException.ThrowIfNullOrWhiteSpace(adminCreateDto.Password, nameof(adminCreateDto.Password));
    }
}