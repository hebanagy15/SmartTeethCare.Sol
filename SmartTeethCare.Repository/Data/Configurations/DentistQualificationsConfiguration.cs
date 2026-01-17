using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartTeethCare.Core.Entities;

namespace SmartTeethCare.Repository.Configurations
{
    public class DentistQualificationsConfiguration : IEntityTypeConfiguration<DentistQualifications>
    {
        public void Configure(EntityTypeBuilder<DentistQualifications> builder)
        {
            builder.ToTable("DentistQualifications");

            // Primary Key
            builder.HasKey(dq => dq.Id);

            // Relationship with Doctor (1:N)
            builder.HasOne(dq => dq.doctor)
                   .WithMany(d => d.DentistQualifications)
                   .HasForeignKey(dq => dq.DoctorId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
