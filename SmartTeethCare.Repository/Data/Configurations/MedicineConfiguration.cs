using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartTeethCare.Core.Entities;

namespace SmartTeethCare.Repository.Configurations
{
    public class MedicineConfiguration : IEntityTypeConfiguration<Medicine>
    {
        public void Configure(EntityTypeBuilder<Medicine> builder)
        {
            builder.ToTable("Medicines");

            // Primary Key
            builder.HasKey(m => m.Id);

            // Properties
            builder.Property(m => m.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(m => m.Description)
                   .HasMaxLength(500);

            builder.Property(m => m.Price)
                   .HasColumnType("decimal(18,2)");

            // Relationships
            builder.HasMany(m => m.PrescriptionMedicines)
                   .WithOne(pm => pm.Medicine)
                   .HasForeignKey(pm => pm.MedicineID)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(m => m.PharmacyMedicines)
                   .WithOne(pm => pm.Medicine)
                   .HasForeignKey(pm => pm.MedicineID)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
