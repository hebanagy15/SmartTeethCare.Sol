using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartTeethCare.Core.DTOs.PatientModule;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Interfaces.Services.PatientModule;
using SmartTeethCare.Core.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Service.PatientModule
{
    public class PatientProfileService : IPatientProfileService
    {
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _environment;
        public PatientProfileService(UserManager<User> userManager , IUnitOfWork unitOfWork , IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _environment = environment;
        }

        public async Task<PatientProfileDto> GetProfileAsync(string userId)
        {
            var patient = (await _unitOfWork.Repository<Patient>()
                .FindAsync(
                    p => p.UserId == userId,
                    q => q.Include(p => p.User)))
                .FirstOrDefault();

            if (patient == null)
                throw new Exception("Patient not found");

            var user = patient.User;

            var fullName = user.DisplayName ?? user.UserName ?? "";
            var names = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            return new PatientProfileDto
            {
                FirstName = names.Length > 0 ? names[0] : "",
                LastName = names.Length > 1 ? names[1] : "",

                Email = user.Email,
                Phone = user.PhoneNumber,

                Address = user.Address,
                Gender = user.Gender,

                DateOfBirth = user.DateOfBirth,

                ProfileImage = patient.ProfileImageUrl 
            };
        }

        public async Task UpdateProfileAsync(string userId, UpdatePatientProfileDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                throw new Exception("User not found");

            var patient = (await _unitOfWork.Repository<Patient>()
                .FindAsync(p => p.UserId == userId))
                .FirstOrDefault();

            if (patient == null)
                throw new Exception("Patient not found");

            var fullName = $"{dto.FirstName} {dto.LastName}".Trim();

            user.DisplayName = fullName;
            user.UserName = fullName.Replace(" ", "").ToLower();

            user.PhoneNumber = dto.Phone;
            user.Address = dto.Address;
            user.Gender = dto.Gender;
            user.DateOfBirth = dto.DateOfBirth;
            user.UpdatedAt = DateTime.Now;

            // رفع الصورة
            if (dto.ProfileImage != null && dto.ProfileImage.Length > 0)
            {
                var folder = Path.Combine(_environment.WebRootPath, "ProfileImages");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var fileName = Guid.NewGuid() + Path.GetExtension(dto.ProfileImage.FileName);
                var filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ProfileImage.CopyToAsync(stream);
                }

                patient.ProfileImageUrl = "/ProfileImages/" + fileName;
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception(errors);
            }

            await _unitOfWork.CompleteAsync();
        }
    }
}


