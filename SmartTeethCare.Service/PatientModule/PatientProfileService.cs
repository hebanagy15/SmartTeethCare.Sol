using Microsoft.AspNetCore.Identity;
using SmartTeethCare.Core.DTOs.PatientModule;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Interfaces.Services.PatientModule;

namespace SmartTeethCare.Service.PatientModule
{
    public class PatientProfileService : IPatientProfileService
    {
        private readonly UserManager<User> _userManager;

        public PatientProfileService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<PatientProfileDto> GetProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                throw new Exception("User not found");

            var fullName = user.DisplayName ?? user.UserName ?? "";
            var names = fullName.Split(' ');

            return new PatientProfileDto
            {
                FirstName = names.Length > 0 ? names[0] : "",
                LastName = names.Length > 1 ? names[1] : "",

                Email = user.Email,
                Phone = user.PhoneNumber,

                Address = user.Address,
                Gender = user.Gender,

                DateOfBirth = user.DateOfBirth
            };
        }

        public async Task UpdateProfileAsync(
            string userId,
            UpdatePatientProfileDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                throw new Exception("User not found");

            var fullName =
                $"{dto.FirstName} {dto.LastName}".Trim();

            user.DisplayName = fullName;
            user.UserName = fullName.Replace(" ", "").ToLower();

            user.PhoneNumber = dto.Phone;
            user.Address = dto.Address;

            user.Gender = dto.Gender;
            user.DateOfBirth = dto.DateOfBirth;

            user.UpdatedAt = DateTime.Now;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception(errors);
            }
            if (!result.Succeeded)
                throw new Exception("Failed to update profile");
        }
    }
}
