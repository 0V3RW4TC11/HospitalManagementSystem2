using System.Linq.Expressions;

namespace HospitalManagementSystem2.Models.Entities;

public class Admin : Entity
{
    public string? Title { get; set; }

    public string FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Gender { get; set; }

    public string? Address { get; set; }

    public string Phone { get; set; }

    public string Email { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public static Expression<Func<Admin, bool>> Matches(Admin admin)
    {
        return x => x.Email.ToLower() == admin.Email.ToLower();
    }
}