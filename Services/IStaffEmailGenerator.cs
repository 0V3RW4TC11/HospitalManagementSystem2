using HospitalManagementSystem2.Models.Entities;

namespace HospitalManagementSystem2.Services
{
    public interface IStaffEmailGenerator
    {
        Task<string> GenerateEmailAsync(Person person, string domain);
    }
}
