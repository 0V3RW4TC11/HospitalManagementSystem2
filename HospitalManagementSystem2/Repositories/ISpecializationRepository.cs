using HospitalManagementSystem2.Models.Entities;

namespace HospitalManagementSystem2.Repositories;

public interface ISpecializationRepository
{
    IQueryable<Specialization> Specializations { get; }
    
    Task AddAsync(Specialization specialization);
    
    Task UpdateAsync(Specialization specialization);
    
    Task RemoveAsync(Specialization specialization);
}