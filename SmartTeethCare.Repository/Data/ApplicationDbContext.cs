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
	public DbSet<User> Users { get; set; }
	public DbSet<Doctor> Doctors { get; set; }
	public DbSet<Pharmacy> Pharmacies { get; set; }
	public DbSet<Patient> Patients { get; set; }


	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		// Apply all configurations from the current assembly
		base.OnModelCreating(modelBuilder);
		modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

	}

}
