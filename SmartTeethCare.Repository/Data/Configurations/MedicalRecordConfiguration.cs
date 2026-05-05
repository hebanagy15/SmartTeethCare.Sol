using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartTeethCare.Core.Entities;

namespace SmartTeethCare.Repository.Configurations
{
    public class MedicalRecordConfiguration : IEntityTypeConfiguration<MedicalRecord>
    {
        public void Configure(EntityTypeBuilder<MedicalRecord> builder)
        {
          
            builder.ToTable("MedicalRecords");

           
            builder.HasKey(m => m.Id);

          
            // Patient relation (1:N)
          
            builder.HasOne(m => m.Patient)
                   .WithMany(p => p.MedicalRecords)
                   .HasForeignKey(m => m.PatientId)
                   .OnDelete(DeleteBehavior.Restrict);

        
            // Doctor relation (1:N)
     
            builder.HasOne(m => m.Doctor)
                   .WithMany(d => d.MedicalRecords)
                   .HasForeignKey(m => m.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict);

            
            // Appointment relation (1:1)
     
            builder.HasOne(m => m.Appointment)
                   .WithOne(a => a.MedicalRecord)
                   .HasForeignKey<MedicalRecord>(m => m.AppointmentId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.SetNull);

         
            builder.Property(m => m.Diagnosis)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(m => m.Notes)
                   .HasMaxLength(2000);

            builder.Property(m => m.CreatedAt)
                   .IsRequired();
        }
    }
}