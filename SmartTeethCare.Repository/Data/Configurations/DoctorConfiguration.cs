using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartTeethCare.Core.Entities;

namespace SmartTeethCare.Repository.Configurations
{
    public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            // Table name
            builder.ToTable("Doctors");

            // Primary Key
            builder.HasKey(d => d.Id);

            // Relationship with User (1:1)
            builder.HasOne(d => d.User)
                   .WithOne(u => u.Doctor)  
                   .HasForeignKey<Doctor>(d => d.UserId)
                   .OnDelete(DeleteBehavior.Restrict);


            // Relationship with Speciality (Many-to-1)
            builder.HasOne(d => d.Speciality)
                   .WithMany() // assuming Speciality has no collection of Doctors
                   .HasForeignKey(d => d.SpecialtyID)
                   .OnDelete(DeleteBehavior.Restrict);

            // Relationship with Prescriptions (1:N)
            builder.HasMany(d => d.Prescriptions)
                   .WithOne(p => p.doctor)
                   .HasForeignKey(p => p.DoctorId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relationship with DentistQualifications (1:N)
            builder.HasMany(d => d.DentistQualifications)
                   .WithOne(q => q.doctor)
                   .HasForeignKey(q => q.DoctorId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relationship with Reviews (1:N)
            builder.HasMany(d => d.Reviews)
                   .WithOne(r => r.doctor)
                   .HasForeignKey(r => r.DentistID)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relationship with Appointments (1:N)
            builder.HasMany(d => d.Appointments)
                   .WithOne(a => a.doctor)
                   .HasForeignKey(a => a.DoctorID)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
