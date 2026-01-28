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

            builder.HasKey(p => p.Id);

            builder.HasOne(p => p.doctor)
                   .WithMany(d => d.Prescriptions)
                   .HasForeignKey(p => p.DoctorId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Patient)
                   .WithMany(pat => pat.Prescriptions)
                   .HasForeignKey(p => p.PatientId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Appointment)
                   .WithMany(a => a.Prescriptions)
                   .HasForeignKey(p => p.AppointmentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.PrescriptionMedicines)
                   .WithOne(pm => pm.Prescription)
                   .HasForeignKey(pm => pm.PrescriptionID)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(p => p.Date)
                   .IsRequired();
        }
    }
}
