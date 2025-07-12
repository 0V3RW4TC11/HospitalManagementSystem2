using HospitalManagementSystem2.Models.Entities;

namespace HospitalManagementSystem2.Repositories;

public interface IAdminRepository
{
    IQueryable<Admin> Admins { get; }
    Task AddAsync(Admin admin);
    Task UpdateAsync(Admin admin);
    Task RemoveAsync(Admin admin);
}