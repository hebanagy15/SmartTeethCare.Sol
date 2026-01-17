using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartTeethCare.Core.Entities;

namespace SmartTeethCare.Repository.Configurations
{
    public class PharmacyConfiguration : IEntityTypeConfiguration<Pharmacy>
    {
        public void Configure(EntityTypeBuilder<Pharmacy> builder)
        {
            builder.ToTable("Pharmacies");

            // Primary Key
            builder.HasKey(p => p.Id);

            // Properties
            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(p => p.Address)
                   .HasMaxLength(500);

            builder.Property(p => p.Location)
                   .HasMaxLength(200);

            builder.Property(p => p.Phone)
                   .HasMaxLength(20);

            builder.Property(p => p.WorkingHours)
                   .HasMaxLength(100);

            // Relationship with PharmacyMedicines (1:N)
            builder.HasMany(p => p.PharmacyMedicines)
                   .WithOne(pm => pm.Pharmacy)
                   .HasForeignKey(pm => pm.PharmacyID)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
