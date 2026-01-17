using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartTeethCare.Core.Entities;

namespace SmartTeethCare.Repository.Configurations
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.ToTable("Patients");

            // Primary Key
            builder.HasKey(p => p.Id);

            // Properties
            builder.Property(p => p.MedicalHistory)
                   .HasMaxLength(1000);

            builder.Property(p => p.UserId)
                   .IsRequired();

            // Relationship with User (1:1)
            builder.HasOne(p => p.User)
                   .WithOne(u => u.Patient)
                   .HasForeignKey<Patient>(p => p.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Relationship with Prescriptions (1:N)
            builder.HasMany(p => p.Prescriptions)
                   .WithOne(pr => pr.Patient)
                   .HasForeignKey(pr => pr.PatientId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relationship with Appointments (1:N)
            builder.HasMany(p => p.Appointments)
                   .WithOne(a => a.patient)
                   .HasForeignKey(a => a.PatientID)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relationship with Reviews (1:N)
            builder.HasMany(p => p.Reviews)
                   .WithOne(r => r.Patient)
                   .HasForeignKey(r => r.PatientID)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
