using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartTeethCare.Core.Entities;

namespace SmartTeethCare.Repository.Data.Configurations
{
    public class SlotReservationConfiguration
        : IEntityTypeConfiguration<SlotReservation>
    {
        public void Configure(EntityTypeBuilder<SlotReservation> builder)
        {
            // Unique Constraint - منع حجز نفس الـ Slot مرتين
            builder.HasIndex(r => new { r.DoctorId, r.Date, r.StartTime })
                   .IsUnique();

            builder.Property(r => r.PaymentIntentId)
                   .IsRequired();

            builder.Property(r => r.ExpiresAt)
                   .IsRequired();
        }
    }
}