using System.Linq.Expressions;

namespace Domain.Entities;

public class Doctor : Entity
{
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public string Gender { get; set; }
    
    public string Address { get; set; }
    
    public string Phone { get; set; }
    
    public string Email { get; set; }
    
    public DateOnly DateOfBirth { get; set; }
    
    public static Expression<Func<Doctor, bool>> Matches(Doctor doctor)
    {
        return x => x.Email.ToLower() == doctor.Email.ToLower();
    }
}