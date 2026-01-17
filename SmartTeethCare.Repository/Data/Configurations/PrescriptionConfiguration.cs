using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartTeethCare.Core.Entities;

namespace SmartTeethCare.Repository.Configurations
{
    public class PrescriptionConfiguration : IEntityTypeConfiguration<Prescription>
    {
        public void Configure(EntityTypeBuilder<Prescription> builder)
        {
            builder.ToTable("Prescriptions");

            // Primary Key
            builder.HasKey(p => p.Id);

            // Relationship with Doctor (Many-to-One)
            builder.HasOne(p => p.doctor)
                   .WithMany(d => d.Prescriptions)
                   .HasForeignKey(p => p.DoctorId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relationship with Patient (Many-to-One)
            builder.HasOne(p => p.Patient)
                   .WithMany(pat => pat.Prescriptions)
                   .HasForeignKey(p => p.PatientId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relationship with PrescriptionMedicines (One-to-Many)
            builder.HasMany(p => p.PrescriptionMedicines)
                   .WithOne(pm => pm.Prescription)
                   .HasForeignKey(pm => pm.PrescriptionID)
                   .OnDelete(DeleteBehavior.Cascade);

            // Property configurations
            builder.Property(p => p.Date)
                   .IsRequired();
        }
    }
}
