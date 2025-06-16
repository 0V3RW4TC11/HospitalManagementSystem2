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

        public DbSet<Person> Persons { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<DoctorSpecialization> DoctorSpecializations { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Attendance> Attendances { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Call base method to configure Identity entities
            base.OnModelCreating(builder);

            // Staff table
            
            builder.Entity<Staff>()
                .HasDiscriminator()
                .HasValue<Admin>(Constants.AuthRoles.Admin)
                .HasValue<Doctor>(Constants.AuthRoles.Doctor);
            builder.Entity<Staff>()
                .HasOne(x => x.Account)
                .WithOne()
                .HasForeignKey<Staff>(x => x.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            // Account table
            
            builder.Entity<Account>()
                .HasOne(x => x.Person)
                .WithOne()
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Account>()
                .HasOne(x => x.User)
                .WithOne()
                .OnDelete(DeleteBehavior.Restrict);

            // Patient table

            builder.Entity<Patient>()
                .HasOne(x => x.Account)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            // Guest table

            builder.Entity<Guest>()
                .HasOne(x => x.Person)
                .WithOne()
                .OnDelete(DeleteBehavior.Restrict);

            // Appointment table

            builder.Entity<Appointment>()
                .HasOne(x => x.Patient)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Appointment>()
               .HasOne(x => x.Doctor)
               .WithMany()
               .OnDelete(DeleteBehavior.Restrict);

            // Attendance table

            builder.Entity<Attendance>()
                .HasOne(x => x.Appointment)
                .WithOne()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
