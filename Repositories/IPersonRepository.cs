using HospitalManagementSystem2.Models.Entities;

namespace HospitalManagementSystem2.Repositories
{
    public interface IPersonRepository
    {
        Task CreateAsync(Person person);

        Task<Person?> FindByIdAsync(Guid id);

        Task UpdateAsync(Person person);

        Task DeleteAsync(Person person);

        IQueryable<Person> Persons { get; }
    }
}
