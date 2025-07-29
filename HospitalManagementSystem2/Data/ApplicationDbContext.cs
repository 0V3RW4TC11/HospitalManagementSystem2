using HospitalManagementSystem2.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace HospitalManagementSystem2.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>, IDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Attendance> Attendances { get; set; }

    public DbSet<Admin> Admins { get; set; }

    public DbSet<Doctor> Doctors { get; set; }

    public DbSet<Patient> Patients { get; set; }

    public DbSet<Specialization> Specializations { get; set; }

    public DbSet<DoctorSpecialization> DoctorSpecializations { get; set; }

    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Account> Accounts { get; set; }

    public async Task SaveChangesAsync()
    {
        await base.SaveChangesAsync();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await base.Database.BeginTransactionAsync();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Call base method to configure Identity entities
        base.OnModelCreating(builder);

        // Configure rules for Account
        builder.Entity<Account>()
            .HasOne(x => x.IdentityUser)
            .WithOne();

        // Configure rules for Appointment
        builder.Entity<Appointment>()
            .HasOne(x => x.Patient)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Appointment>()
            .HasOne(x => x.Doctor)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        // Configure rules for Attendance
        builder.Entity<Attendance>()
            .HasOne(x => x.Appointment)
            .WithOne()
            .OnDelete(DeleteBehavior.Restrict);

        // Configure rules for DoctorSpecialization
        builder.Entity<DoctorSpecialization>()
            .HasOne(x => x.Specialization)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);
        builder.Entity<DoctorSpecialization>()
            .HasOne(x => x.Doctor)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);
    }
}