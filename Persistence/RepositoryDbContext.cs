using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public sealed class RepositoryDbContext : IdentityDbContext<IdentityUser>
{
    public RepositoryDbContext(DbContextOptions<RepositoryDbContext> options) : base(options)
    {
    }
    
    public DbSet<Attendance> Attendances { get; set; }

    public DbSet<Admin> Admins { get; set; }

    public DbSet<Doctor> Doctors { get; set; }

    public DbSet<Patient> Patients { get; set; }

    public DbSet<Specialization> Specializations { get; set; }

    public DbSet<DoctorSpecialization> DoctorSpecializations { get; set; }

    public DbSet<Account> Accounts { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder) => 
        builder.ApplyConfigurationsFromAssembly(typeof(RepositoryDbContext).Assembly);
}