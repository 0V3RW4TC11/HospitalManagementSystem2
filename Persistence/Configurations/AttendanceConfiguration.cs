using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

internal sealed class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
{
    public void Configure(EntityTypeBuilder<Attendance> builder)
    {
        builder.Property(a => a.PatientId)
            .IsRequired();

        builder.Property(a => a.DoctorId)
            .IsRequired();

        builder.HasOne<Patient>()
            .WithMany()
            .HasForeignKey(p => p.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Doctor>()
            .WithMany()
            .HasForeignKey(p => p.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
