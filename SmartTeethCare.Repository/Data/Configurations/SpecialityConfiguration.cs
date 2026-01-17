using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartTeethCare.Core.Entities;

namespace SmartTeethCare.Repository.Configurations
{
    public class SpecialityConfiguration : IEntityTypeConfiguration<Speciality>
    {
        public void Configure(EntityTypeBuilder<Speciality> builder)
        {
            builder.ToTable("Specialities");

            // Primary Key
            builder.HasKey(s => s.Id);

            // Relationship with Doctors (1:N)
            builder.HasMany(s => s.doctors)
                   .WithOne(d => d.Speciality)
                   .HasForeignKey(d => d.SpecialtyID)
                   .OnDelete(DeleteBehavior.Restrict);

            // Relationship with EducationalVideos (1:N)
            builder.HasMany(s => s.EducationalVideos)
                   .WithOne(ev => ev.Speciality)
                   .HasForeignKey(ev => ev.SpecialtyID)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
