using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Repository.Data;

namespace SmartTeethCare.Repository.DataSeed
{
    public static class DoctorSeed
    {
        public static async Task SeedDoctorsAsy(UserManager<User> userManager,ApplicationDbContext context)
        {
            
            var specialities = await context.Specialities.ToListAsync();

            if (!specialities.Any())
                return;

            var doctors = new List<(string Name, string Gender, DateTime Birth, int Salary, int Hours, int Exp, int SpecIndex)>
        {
            ("Ahmed Hassan", "Male", new DateTime(1988,5,10), 12000, 8, 6, 5),
            ("Mona Samir", "Female", new DateTime(1990,3,22), 11500, 7, 5, 5),

            ("Khaled Mahmoud", "Male", new DateTime(1985,11,2), 13000, 8, 8, 1),
            ("Nour Ali", "Female", new DateTime(1992,7,15), 11000, 6, 4, 1),

            ("Tarek Fathy", "Male", new DateTime(1987,1,18), 12500, 8, 7, 2),
            ("Salma Hany", "Female", new DateTime(1993,9,9), 10500, 6, 3, 2),

            ("Omar Adel", "Male", new DateTime(1989,12,5), 11800, 7, 5, 3),
            ("Farah Mostafa", "Female", new DateTime(1994,4,12), 10200, 6, 2, 3),

            ("Youssef Nabil", "Male", new DateTime(1986,6,30), 13500, 9, 9, 4),
            ("Heba Magdy", "Female", new DateTime(1991,2,14), 11200, 7, 4, 4),

            ("Mahmoud Samy", "Male", new DateTime(1989,8,20), 12000, 8, 5, 0),
            ("Sara Fawzy", "Female", new DateTime(1992,10,11), 11000, 7, 4, 0),

            ("Adel Hossam", "Male", new DateTime(1987,3,3), 12500, 8, 6, 1),
            ("Lina Khalil", "Female", new DateTime(1991,6,25), 10800, 7, 3, 1),

            ("Hassan Youssef", "Male", new DateTime(1988,12,14), 12300, 8, 5, 2),
            ("Dina Magdy", "Female", new DateTime(1993,5,5), 10400, 6, 2, 2),

            ("Amr Mostafa", "Male", new DateTime(1986,9,9), 13000, 9, 7, 3),
            ("Reem Salah", "Female", new DateTime(1990,1,30), 10600, 7, 3, 3),

            ("Omar Fouad", "Male", new DateTime(1985,11,11), 13200, 9, 8, 4),
            ("Mariam Gamal", "Female", new DateTime(1992,4,4), 10900, 7, 4, 4),
        };

            foreach (var d in doctors)
            {
                var speciality = specialities.ElementAtOrDefault(d.SpecIndex);
                if (speciality == null)
                    continue;

                string email = $"{d.Name.Replace(" ", "").ToLower()}@smartteeth.com";

                var existingUser = await userManager.FindByEmailAsync(email);
                if (existingUser != null)
                    continue;

                var user = new User
                {
                    UserName = email,
                    Email = email,
                    Address = "Cairo, Egypt",
                    Gender = d.Gender,
                    DateOfBirth = d.Birth,
                    EmailConfirmed = true,
                    
                };

                await userManager.CreateAsync(user, "Doctor@123");
                await userManager.AddToRoleAsync(user, "Doctor");

                var doctor = new Doctor
                {
                    UserId = user.Id,
                    Salary = d.Salary,
                    WorkingHours = d.Hours,
                    HiringDate = DateTime.Now.AddYears(-d.Exp),
                    SpecialtyID = speciality.Id
                };

                context.Doctors.Add(doctor);
            }

            await context.SaveChangesAsync();
        }
    }

}
