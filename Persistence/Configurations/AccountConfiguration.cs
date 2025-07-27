using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(a => a.UserId);
        
        builder.Property(a => a.IdentityUserId)
            .IsRequired();

        builder.HasOne<IdentityUser>()
            .WithOne()
            .HasForeignKey<Account>(a => a.IdentityUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}