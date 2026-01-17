using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartTeethCare.Core.Entities;

namespace SmartTeethCare.Repository.Configurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.ToTable("Reviews");

            // Primary Key
            builder.HasKey(r => r.Id);

            // Relationship with Doctor (Many-to-One)
            builder.HasOne(r => r.doctor)
                   .WithMany(d => d.Reviews)
                   .HasForeignKey(r => r.DentistID)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relationship with Patient (Many-to-One)
            builder.HasOne(r => r.Patient)
                   .WithMany(p => p.Reviews)
                   .HasForeignKey(r => r.PatientID)
                   .OnDelete(DeleteBehavior.Cascade);

            // Optional: Set default value for CreatedAt
            builder.Property(r => r.CreatedAt)
                   .HasDefaultValueSql("GETDATE()");
        }
    }
}
