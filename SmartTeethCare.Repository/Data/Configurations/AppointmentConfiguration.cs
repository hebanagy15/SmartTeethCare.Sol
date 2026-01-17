using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartTeethCare.Core.Entities;

namespace SmartTeethCare.Repository.Configurations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.ToTable("Appointments");

            // Primary Key
            builder.HasKey(a => a.Id);

            // Relationship with Doctor (1:N)
            builder.HasOne(a => a.doctor)
                   .WithMany(d => d.Appointments)
                   .HasForeignKey(a => a.DoctorID)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relationship with Patient (1:N)
            builder.HasOne(a => a.patient)
                   .WithMany(p => p.Appointments)
                   .HasForeignKey(a => a.PatientID)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
