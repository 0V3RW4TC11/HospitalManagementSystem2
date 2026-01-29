// TODO: Remove this in favor of using Asp Net Identity user claims

namespace Domain.Entities;

public class Account
{
    public Guid UserId { get; set; } // Give this to Asp Identity in a user claim, get it back using Queries

    public string IdentityUserId { get; set; } // Leaky abstraction, Asp Identity uses string guids but are other systems the same?
}