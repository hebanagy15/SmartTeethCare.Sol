using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartTeethCare.Core.Entities;

namespace SmartTeethCare.Repository.Configurations
{
    public class AdminConfiguration : IEntityTypeConfiguration<Admin>
    {
        public void Configure(EntityTypeBuilder<Admin> builder)
        {
            builder.ToTable("Admins");

            builder.HasKey(a => a.Id);

            // Relation with User
            builder.HasOne(a => a.User)
           .WithOne(u => u.Admin)        // <-- navigation في User
           .HasForeignKey<Admin>(a => a.UserId)
           .IsRequired(false);
        }
    }
}
