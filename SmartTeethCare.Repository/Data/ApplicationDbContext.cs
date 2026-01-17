using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartTeethCare.Core.Entities;
using System.Reflection;

namespace SmartTeethCare.Repository.Data;

public class ApplicationDbContext : IdentityDbContext<User>
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
		: base(options)
	{
	}

	// Define DbSets for your entities here
	public DbSet<Doctor> Doctors { get; set; }
	public DbSet<Pharmacy> Pharmacies { get; set; }
	public DbSet<Patient> Patients { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<PrescriptionMedicine> PrescriptionMedicines { get; set; }
    public DbSet<Medicine> Medicines { get; set; }
    public DbSet<DentistQualifications> DentistQualifications { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Speciality> Specialities { get; set; }
    public DbSet<PharmacyMedicine> PharmacyMedicines { get; set; }
    public DbSet<EducationalVideos> EducationalVideos { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }






    protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		// Apply all configurations from the current assembly
		base.OnModelCreating(modelBuilder);
		modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

	}

}
