using HospitalManagementSystem2.Data;
using HospitalManagementSystem2.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Security.Principal;

namespace HospitalManagementSystem2.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<Person> _persons;

        public PersonRepository(ApplicationDbContext context)
        {
            _context = context;
            _persons = context.Persons;
        }

        public IQueryable<Person> Persons => _persons.AsNoTracking();

        public async Task CreateAsync(Person person)
        {
            await _persons.AddAsync(person);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Person person)
        {
            _persons.Remove(person);
            await _context.SaveChangesAsync();
        }

        public async Task<Person?> FindByIdAsync(Guid id)
        {
            return await _persons.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateAsync(Person person)
        {
            var entry = _context.Entry(person);
            entry.State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
