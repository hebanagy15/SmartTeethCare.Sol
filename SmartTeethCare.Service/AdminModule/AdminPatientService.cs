using Microsoft.AspNetCore.Identity;
using SmartTeethCare.Core.DTOs.AdminModule;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Interfaces.Services.AdminModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Service.AdminModule
{
    public class AdminPatientService : IAdminPatientService
    {
        private readonly UserManager<User> _userManager;

        public AdminPatientService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<string> CreatePatientAsync(CreatePatientByAdminDTO dto)
        {
            var tempPassword = $"Pt@{Guid.NewGuid().ToString()[..8]}";

            var patient = new User
            {
                UserName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(patient, tempPassword);

            if (!result.Succeeded)
                throw new Exception(result.Errors.First().Description);

            await _userManager.AddToRoleAsync(patient, "Patient");

            return tempPassword; 
        }
    }

}
