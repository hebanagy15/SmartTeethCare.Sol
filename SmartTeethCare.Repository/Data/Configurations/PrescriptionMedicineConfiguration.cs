using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartTeethCare.Core.Entities;

namespace SmartTeethCare.Repository.Configurations
{
    public class PrescriptionMedicineConfiguration : IEntityTypeConfiguration<PrescriptionMedicine>
    {
        public void Configure(EntityTypeBuilder<PrescriptionMedicine> builder)
        {
            builder.ToTable("PrescriptionMedicines");

            // Composite Key
            builder.HasKey(pm => new { pm.PrescriptionID, pm.MedicineID });

            // Relationship with Prescription (Many-to-One)
            builder.HasOne(pm => pm.Prescription)
                   .WithMany(p => p.PrescriptionMedicines)
                   .HasForeignKey(pm => pm.PrescriptionID)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relationship with Medicine (Many-to-One)
            builder.HasOne(pm => pm.Medicine)
                   .WithMany(m => m.PrescriptionMedicines)
                   .HasForeignKey(pm => pm.MedicineID)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
