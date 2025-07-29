using System.Linq.Expressions;

namespace Domain.Entities;

public class Patient : Entity
{
    public string? Title { get; set; }

    public string FirstName { get; set; }

    public string? LastName { get; set; }

    public string Gender { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string Email { get; set; }

    public DateOnly DateOfBirth { get; set; }
    
    public BloodType BloodType { get; set; }
    
    public static Expression<Func<Patient, bool>> Matches(Patient patient)
    {
        return x => x.Email.ToLower() == patient.Email.ToLower();
    }
}

public enum BloodType
{
    APositive,
    ANegative,
    BPositive,
    BNegative,
    AbPositive,
    AbNegative,
    OPositive,
    ONegative
}