using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartTeethCare.Core.DTOs.AdminModule;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Interfaces.Services.AdminModule;
using SmartTeethCare.Core.Interfaces.UnitOfWork;
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
        private readonly IUnitOfWork _unitOfWork;

        public AdminPatientService(UserManager<User> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
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

        public async Task<IEnumerable<AdminPatientDto>> GetAllAsync()
        {
            var patients = await _unitOfWork.Repository<Patient>()
                .FindAsync(
                    p => true,
                    q => q.Include(p => p.User));

            return patients.Select(p => new AdminPatientDto
            {
                Id = p.Id,
                PatientName = p.User.DisplayName ?? p.User.UserName,
                Email = p.User.Email,
                Phone = p.User.PhoneNumber,
                Gender = p.User.Gender,
                ProfileImageUrl = p.ProfileImageUrl
            });
        }

        public async Task<AdminPatientDto> GetByIdAsync(int id)
        {
            var patient = (await _unitOfWork.Repository<Patient>()
                .FindAsync(
                    p => p.Id == id,
                    include: q => q.Include(p => p.User)))
                .FirstOrDefault();

            if (patient == null)
                throw new Exception("Patient not found.");

            return new AdminPatientDto
            {
                Id = patient.Id,
                PatientName = string.IsNullOrWhiteSpace(patient.User.DisplayName)
                    ? patient.User.UserName
                    : patient.User.DisplayName,

                Email = patient.User.Email,
                Phone = patient.User.PhoneNumber,
                Gender = patient.User.Gender,
                ProfileImageUrl = patient.ProfileImageUrl
            };
        }
    }

}
