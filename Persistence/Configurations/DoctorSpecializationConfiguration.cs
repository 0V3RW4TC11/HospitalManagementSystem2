using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class DoctorSpecializationConfiguration : IEntityTypeConfiguration<DoctorSpecialization>
{
    public void Configure(EntityTypeBuilder<DoctorSpecialization> builder)
    {
        builder.HasKey(ds => new {ds.DoctorId, ds.SpecializationId});

        builder.HasOne<Doctor>()
            .WithMany()
            .HasForeignKey(ds => ds.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne<Specialization>()
            .WithMany()
            .HasForeignKey(ds => ds.SpecializationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}