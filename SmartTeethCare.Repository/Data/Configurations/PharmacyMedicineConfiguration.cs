using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartTeethCare.Core.Entities;

namespace SmartTeethCare.Repository.Configurations
{
    public class PharmacyMedicineConfiguration : IEntityTypeConfiguration<PharmacyMedicine>
    {
        public void Configure(EntityTypeBuilder<PharmacyMedicine> builder)
        {
            builder.ToTable("PharmacyMedicines");

            // Composite Primary Key
            builder.HasKey(pm => new { pm.PharmacyID, pm.MedicineID });

            // Property
            builder.Property(pm => pm.StockQuantity)
                   .IsRequired();

            // Relationship with Pharmacy (Many-to-One)
            builder.HasOne(pm => pm.Pharmacy)
                   .WithMany(p => p.PharmacyMedicines)
                   .HasForeignKey(pm => pm.PharmacyID)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relationship with Medicine (Many-to-One)
            builder.HasOne(pm => pm.Medicine)
                   .WithMany(m => m.PharmacyMedicines)
                   .HasForeignKey(pm => pm.MedicineID)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
