using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Repository.Data;
using System;
using System.Threading.Tasks;

namespace SmartTeethCare.Repository.DataSeed
{
    public static class AppRoles
    {
        public const string Admin = "Admin";
        public const string Doctor = "Doctor";
        public const string Patient = "Patient";
    }

    public static class SeedUsers
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            var logger = loggerFactory?.CreateLogger("SeedUsers");

            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // ---- 1️⃣ Seed Roles ----
            string[] roles = { AppRoles.Admin, AppRoles.Doctor, AppRoles.Patient };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole(role));
                    if (!roleResult.Succeeded)
                    {
                        foreach (var err in roleResult.Errors)
                            logger?.LogError("Failed to create role {Role}: {Error}", role, err.Description);
                    }
                }
            }

            // ---- 2️⃣ Seed Admin ----
            var adminEmail = "admin@site.test";
            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new User
                {
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed = true,
                    Address = "Admin address",
                    Gender = "NotSpecified",
                    DateOfBirth = DateTime.Parse("1990-01-01"),
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                var createAdmin = await userManager.CreateAsync(admin, "P@ssw0rd!1"); // DEV ONLY
                if (createAdmin.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, AppRoles.Admin);
                    logger?.LogInformation("Admin user created: {Email}", adminEmail);
                }
                else
                {
                    foreach (var err in createAdmin.Errors)
                        logger?.LogError(err.Description);
                }
            }

            // ---- 3️⃣ Seed Doctor ----
            var doctorEmail = "doctor@test.com";
            var doctorUser = await userManager.FindByEmailAsync(doctorEmail);
            if (doctorUser == null)
            {
                doctorUser = new User
                {
                    UserName = "doctor1",
                    Email = doctorEmail,
                    EmailConfirmed = true,
                    Address = "Cairo, Egypt",
                    Gender = "Male",
                    DateOfBirth = DateTime.Parse("1985-05-10"),
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                var createDoc = await userManager.CreateAsync(doctorUser, "Doctor@123");
                if (createDoc.Succeeded)
                {
                    await userManager.AddToRoleAsync(doctorUser, AppRoles.Doctor);
                    logger?.LogInformation("Doctor user created: {Email}", doctorEmail);

                    // Create Doctor entity if not exists
                    var existingDoctor = await dbContext.Doctors
                        .FirstOrDefaultAsync(d => d.UserId == doctorUser.Id);

                    if (existingDoctor == null)
                    {
                        var doctorEntity = new Doctor
                        {
                            Salary = 10000,
                            WorkingHours = 40,
                            HiringDate = DateTime.Now.Date,
                            UserId = doctorUser.Id
                        };
                        dbContext.Doctors.Add(doctorEntity);
                        await dbContext.SaveChangesAsync();
                        logger?.LogInformation("Doctor entity created for user {UserId}", doctorUser.Id);
                    }
                }
                else
                {
                    foreach (var err in createDoc.Errors)
                        logger?.LogError(err.Description);
                }
            }

            // ---- 4️⃣ Seed Patient ----
            var patientEmail = "patient@test.com";
            var patientUser = await userManager.FindByEmailAsync(patientEmail);
            if (patientUser == null)
            {
                patientUser = new User
                {
                    UserName = "patient1",
                    Email = patientEmail,
                    EmailConfirmed = true,
                    Address = "Giza, Egypt",
                    Gender = "Female",
                    DateOfBirth = DateTime.Parse("2000-03-20"),
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                var createPatient = await userManager.CreateAsync(patientUser, "Patient@123");
                if (createPatient.Succeeded)
                {
                    await userManager.AddToRoleAsync(patientUser, AppRoles.Patient);
                    logger?.LogInformation("Patient user created: {Email}", patientEmail);

                    // Create Patient entity if not exists
                    var existingPatient = await dbContext.Patients
                        .FirstOrDefaultAsync(p => p.UserId == patientUser.Id);

                    if (existingPatient == null)
                    {
                        var patientEntity = new Patient
                        {
                            MedicalHistory = "No major history",
                            UserId = patientUser.Id
                        };
                        dbContext.Patients.Add(patientEntity);
                        await dbContext.SaveChangesAsync();
                        logger?.LogInformation("Patient entity created for user {UserId}", patientUser.Id);
                    }
                }
                else
                {
                    foreach (var err in createPatient.Errors)
                        logger?.LogError(err.Description);
                }
            }
        }
    }
}
