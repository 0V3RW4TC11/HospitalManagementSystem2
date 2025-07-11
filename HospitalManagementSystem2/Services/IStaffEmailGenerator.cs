using HospitalManagementSystem2.Models.Entities;

namespace HospitalManagementSystem2.Services;

public interface IStaffEmailGenerator
{
    Task<string> GenerateEmailAsync(string firstname, string? lastname, string domain);
}