using Services.Dtos.Admin;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using Mapster;
using Services.Abstractions;

namespace Services;

internal sealed class AdminService : IAdminService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly IStaffEmailService _staffEmailService;
    private readonly AccountService _accountHelper;

    public AdminService(
        IRepositoryManager repositoryManager,
        IStaffEmailService staffEmailService,
        AccountService accountHelper
        )
    {
        _repositoryManager = repositoryManager;
        _accountHelper = accountHelper;
        _staffEmailService = staffEmailService;
    }

    public async Task<(AdminDto[] List, int TotalCount)> Admins(int pageNumber, int pageSize)
    {
        var admins = await _repositoryManager.AdminRepository.GetAdmins(pageNumber, pageSize);
        int totalCount = await _repositoryManager.AdminRepository.GetTotalCount();
        var dtos = admins.Select(a => a.Adapt<AdminDto>()).ToArray();

        return (List: dtos, TotalCount: totalCount);
    }

    public async Task<AdminDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var admin = await GetAdminByIdAsync(id);

        return admin.Adapt<AdminDto>();
    }

    public async Task CreateAsync(AdminCreateDto adminCreateDto, CancellationToken cancellationToken = default)
    {
        await ValidateAdminCreateDtoAsync(adminCreateDto);
        
        await _repositoryManager.UnitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var admin = adminCreateDto.Adapt<Admin>();
            _repositoryManager.AdminRepository.Add(admin);
            await _repositoryManager.UnitOfWork.SaveChangesAsync();
            
            var username = await _staffEmailService.CreateStaffEmailAsync(adminCreateDto.FirstName, adminCreateDto.LastName);
            
            await _accountHelper.CreateAsync(admin.Id, Constants.AuthRoles.Admin, username, adminCreateDto.Password);
        });
    }

    public async Task UpdateAsync(AdminDto adminDto, CancellationToken cancellationToken = default)
    {
        var admin = await GetAdminByIdAsync(adminDto.Id);
        
        ValidateAdminDto(adminDto);
        
        admin.Title = adminDto.Title;
        admin.FirstName = adminDto.FirstName;
        admin.LastName = adminDto.LastName;
        admin.Gender = adminDto.Gender;
        admin.Address = adminDto.Address;
        admin.Phone = adminDto.Phone;
        admin.Email = adminDto.Email;
        admin.DateOfBirth = adminDto.DateOfBirth;
        
        await _repositoryManager.UnitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var admin = await GetAdminByIdAsync(id);

        await _repositoryManager.UnitOfWork.ExecuteInTransactionAsync(async () =>
        {
            _repositoryManager.AdminRepository.Remove(admin);
            await _repositoryManager.UnitOfWork.SaveChangesAsync();
            await _accountHelper.DeleteAsync(admin.Id);
        });
    }

    private async Task<Admin> GetAdminByIdAsync(Guid id)
    {
        var admin = await _repositoryManager.AdminRepository.FindByIdAsync(id);
        if (admin is null)
            throw new AdminNotFoundException();
        
        return admin;
    }
    
    private async Task ValidateAdminCreateDtoAsync(AdminCreateDto adminCreateDto)
    {
        if (await IsExistingAsync(adminCreateDto))
            throw new AdminDuplicationException($"Email {adminCreateDto.Email} is used by another Admin.");

        ValidateAdminBaseDto(adminCreateDto);
        ArgumentException.ThrowIfNullOrWhiteSpace(adminCreateDto.Password, nameof(adminCreateDto.Password));
    }
    
    private static void ValidateAdminDto(AdminDto adminDto)
    {
        ValidateAdminBaseDto(adminDto);
    }

    private async Task<bool> IsExistingAsync(AdminCreateDto adminCreateDto)
    {
        return await _repositoryManager.AdminRepository.ExistsAsync(a =>
            a.Email.ToLower() == adminCreateDto.Email.ToLower());
    }

    private static void ValidateAdminBaseDto(AdminBaseDto baseDto)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(baseDto.FirstName, nameof(baseDto.FirstName));
        ArgumentException.ThrowIfNullOrWhiteSpace(baseDto.Phone, nameof(baseDto.Phone));
        ArgumentException.ThrowIfNullOrWhiteSpace(baseDto.Email, nameof(baseDto.Email));
    }
}