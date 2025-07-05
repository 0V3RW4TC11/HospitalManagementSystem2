using HospitalManagementSystem2.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem2.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<DoctorSpecialization> DoctorSpecializations { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Attendance> Attendances { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Call base method to configure Identity entities
            base.OnModelCreating(builder);

            // Configure TPH for Staff hierarchy
            builder.Entity<Staff>()
                .HasDiscriminator()
                .HasValue<Admin>(Constants.AuthRoles.Admin)
                .HasValue<Doctor>(Constants.AuthRoles.Doctor);

            // Configure delete rules for Staff
            builder.Entity<Staff>()
                .HasOne(x => x.User)
                .WithOne()
                .OnDelete(DeleteBehavior.Restrict);

            // Configure delete rules for Patient
            builder.Entity<Patient>()
                .HasOne(x => x.User)
                .WithOne()
                .OnDelete(DeleteBehavior.Restrict);

            // Configure delete rules for Appointment
            builder.Entity<Appointment>()
                .HasOne(x => x.Patient)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Appointment>()
               .HasOne(x => x.Doctor)
               .WithMany()
               .OnDelete(DeleteBehavior.Restrict);

            // Configure delete rules for Attendance
            builder.Entity<Attendance>()
                .HasOne(x => x.Appointment)
                .WithOne()
                .OnDelete(DeleteBehavior.Restrict);

            // Configure delete rules for DoctorSpecialization
            builder.Entity<DoctorSpecialization>()
                .HasOne(x => x.Specialization)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
