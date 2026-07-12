using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SmartTeethCare.Core.DTOs.AdminModule;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Interfaces.Services.AdminModule;
using SmartTeethCare.Core.Interfaces.Services.SecurityModule;
using SmartTeethCare.Core.Interfaces.UnitOfWork;
using SmartTeethCare.Service.SecurityModule;
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
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AdminPatientService(UserManager<User> userManager, IUnitOfWork unitOfWork,IEmailService emailService,IConfiguration configuration)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<string> CreatePatientAsync(CreatePatientByAdminDTO dto)
        {
            var existingEmail = await _userManager.FindByEmailAsync(dto.Email);

            if (existingEmail != null)
                throw new Exception("Email already exists.");

            var existingUserName = await _userManager.FindByNameAsync(dto.Email);

            if (existingUserName != null)
                throw new Exception("Username already exists.");

            var tempPassword = $"Pt@{Guid.NewGuid().ToString()[..8]}";

            var user = new User
            {
                UserName = dto.Email,              // íÝÖá íßæä ÇáÅíãíá
                DisplayName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
                PhoneNumber = dto.PhoneNumber,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, tempPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(Environment.NewLine,
                    result.Errors.Select(e => e.Description));

                throw new Exception(errors);
            }

            await _userManager.AddToRoleAsync(user, "Patient");

            // Generate confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // Encode token for URL
            token = Uri.EscapeDataString(token);

            // Confirmation URL
            var confirmLink =
                $"{_configuration["AppSettings:BaseUrl"]}/api/Account/confirm-email?userId={user.Id}&token={token}";

            // Send confirmation email
            await _emailService.SendTemplateEmailAsync(
                user.Email!,
                "Confirm your email",
                "EmailConfirmation",
                new Dictionary<string, string>
                {
        { "CONFIRM_LINK", confirmLink }
                });
            Console.WriteLine(confirmLink);
            var patient = new Patient
            {
                UserId = user.Id,
                MedicalHistory = string.Empty,
                ProfileImageUrl = null
            };

            await _unitOfWork.Repository<Patient>().AddAsync(patient);
            await _unitOfWork.CompleteAsync();

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
