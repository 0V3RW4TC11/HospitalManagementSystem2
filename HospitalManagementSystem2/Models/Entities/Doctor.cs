using System.Linq.Expressions;

namespace HospitalManagementSystem2.Models.Entities;

public class Doctor : Entity
{
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public string Gender { get; set; }
    
    public string Address { get; set; }
    
    public string Phone { get; set; }
    
    public string Email { get; set; }
    
    public DateOnly DateOfBirth { get; set; }
    
    public IEnumerable<Specialization> Specializations { get; set; }
    
    public static Expression<Func<Doctor, bool>> Matches(Doctor doctor)
    {
        return x => x.Email.ToLower() == doctor.Email.ToLower();
    }
}