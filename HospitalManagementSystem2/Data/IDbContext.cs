using HospitalManagementSystem2.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace HospitalManagementSystem2.Data;

public interface IDbContext
{
    DbSet<Admin> Admins { get; set; }
    DbSet<Doctor> Doctors { get; set; }
    DbSet<Patient> Patients { get; set; }
    DbSet<Specialization> Specializations { get; set; }
    DbSet<DoctorSpecialization> DoctorSpecializations { get; set; }
    DbSet<Appointment> Appointments { get; set; }
    DbSet<Account> Accounts { get; set; }
    
    Task SaveChangesAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
}