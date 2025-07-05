using HospitalManagementSystem2.Models.Entities;

namespace HospitalManagementSystem2.Services
{
    public interface IAdminRepository
    {
        Task CreateAsync(Admin admin, string password);
        Task<Admin?> FindByIdAsync(Guid id);
        Task UpdateAsync(Admin admin);
        Task DeleteAsync(Admin admin);
        IQueryable<Admin> Admins { get; }
    }
}
