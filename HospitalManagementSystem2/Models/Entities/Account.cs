using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem2.Models.Entities;

[PrimaryKey(nameof(UserId), nameof(IdentityUserId))]
public class Account
{
    public Guid UserId { get; set; }
    
    public string IdentityUserId { get; set; }

    public IdentityUser IdentityUser { get; set; }
}