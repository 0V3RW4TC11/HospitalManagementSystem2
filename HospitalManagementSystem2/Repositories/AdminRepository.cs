using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem2.Repositories;

public class AdminRepository : IAdminRepository
{
    private readonly DbSet<Admin> _admins;

    public AdminRepository(IDbContext context)
    {
        _admins = context.Admins;
    }

    public IQueryable<Admin> Admins => _admins.AsNoTracking();

    public async Task AddAsync(Admin admin)
    {
        await _admins.AddAsync(admin);
    }
    
    public async Task UpdateAsync(Admin admin)
    {
        var entity = await _admins.SingleAsync(x => x.Id == admin.Id);
        
        entity.Title = admin.Title;
        entity.FirstName = admin.FirstName;
        entity.LastName = admin.LastName;
        entity.Email = admin.Email;
        entity.Phone = admin.Phone;
        entity.Address = admin.Address;
        entity.DateOfBirth = admin.DateOfBirth;
        entity.Gender = admin.Gender;
        
        _admins.Update(entity);
    }

    public async Task RemoveAsync(Admin admin)
    {
        var entity = await _admins.SingleAsync(x => x.Id == admin.Id);
        _admins.Remove(entity);
    }
}