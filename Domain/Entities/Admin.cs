using System.Linq.Expressions;

namespace Domain.Entities;

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
}