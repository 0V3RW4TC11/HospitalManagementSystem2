using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public sealed class HmsDbContext : IdentityDbContext<IdentityUser>
{
    public HmsDbContext(DbContextOptions<HmsDbContext> options) : base(options)
    {
    }
    
    public DbSet<Attendance> Attendances { get; set; }

    public DbSet<Admin> Admins { get; set; }

    public DbSet<Doctor> Doctors { get; set; }

    public DbSet<Patient> Patients { get; set; }

    public DbSet<Specialization> Specializations { get; set; }

    public DbSet<DoctorSpecialization> DoctorSpecializations { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Configure Identity services
        base.OnModelCreating(builder);
        
        // Configure Db schema from configuration classes
        builder.ApplyConfigurationsFromAssembly(typeof(HmsDbContext).Assembly);
    }
}