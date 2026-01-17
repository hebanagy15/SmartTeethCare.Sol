using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartTeethCare.Core.Entities;

namespace SmartTeethCare.Repository.Configurations
{
    public class EducationalVideosConfiguration : IEntityTypeConfiguration<EducationalVideos>
    {
        public void Configure(EntityTypeBuilder<EducationalVideos> builder)
        {
            builder.ToTable("EducationalVideos");

            // Primary Key
            builder.HasKey(ev => ev.Id);

            // Relationship with Speciality (Many-to-One)
            builder.HasOne(ev => ev.Speciality)
                   .WithMany(s => s.EducationalVideos)
                   .HasForeignKey(ev => ev.SpecialtyID)
                   .OnDelete(DeleteBehavior.Cascade);

            // Property configurations
            builder.Property(ev => ev.VideoURL)
                   .IsRequired()
                   .HasMaxLength(500);
        }
    }
}
