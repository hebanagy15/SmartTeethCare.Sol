using Microsoft.AspNetCore.Identity;
using SmartTeethCare.Core.DTOs.AdminModule;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Interfaces.Services.AdminModule;
using SmartTeethCare.Core.Interfaces.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace SmartTeethCare.Service.AdminModule
{
    public class AdminDoctorService : IAdminDoctorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public AdminDoctorService(IUnitOfWork unitOfWork,
                                  UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync()
        {
            var doctors = await _unitOfWork
                .Repository<Doctor>()
                .FindAsync(include: q => q
                    .Include(d => d.User)
                    .Include(d => d.Speciality));

            return doctors.Select(d => new DoctorDto
            {
                Id = d.Id,
                UserId = d.UserId,
                FullName = d.User.DisplayName ?? d.User.UserName,
                Email = d.User.Email,
                Salary = d.Salary,
                WorkingHours = d.WorkingHours,
                HiringDate = d.HiringDate,
                SpecialityName = d.Speciality?.Name
            });
        }

        public async Task<DoctorDto?> GetDoctorByIdAsync(int id)
        {
            var doctor = (await _unitOfWork
                .Repository<Doctor>()
                .FindAsync(d => d.Id == id,
                           q => q.Include(d => d.User)
                                 .Include(d => d.Speciality)))
                .FirstOrDefault();

            if (doctor == null) return null;

            return new DoctorDto
            {
                Id = doctor.Id,
                UserId = doctor.UserId,
                FullName = doctor.User.DisplayName ?? doctor.User.UserName,
                Email = doctor.User.Email,
                Salary = doctor.Salary,
                WorkingHours = doctor.WorkingHours,
                HiringDate = doctor.HiringDate,
                SpecialityName = doctor.Speciality?.Name
            };
        }

        public async Task AddDoctorAsync(CreateDoctorDto dto)
        {
            var user = new User
            {
                UserName = dto.Email,
                Email = dto.Email,

                DisplayName = dto.FullName.StartsWith("Dr.")
                    ? dto.FullName
                    : $"Dr. {dto.FullName}",

                Address = dto.Address ?? "Not Provided",
                Gender = dto.Gender ?? "Not Specified"
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception(errors);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "Doctor");

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                throw new Exception(errors);
            }

            var doctor = new Doctor
            {
                UserId = user.Id,
                Salary = dto.Salary,
                WorkingHours = dto.WorkingHours,
                HiringDate = dto.HiringDate,
                SpecialtyID = dto.SpecialityID
            };

            await _unitOfWork.Repository<Doctor>().AddAsync(doctor);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateDoctorAsync(int id, UpdateDoctorDto dto)
        {
            var doctor = await _unitOfWork.Repository<Doctor>()
                .FindAsync(d => d.Id == id,
                           include: q => q.Include(d => d.User));

            var doctorEntity = doctor.FirstOrDefault();

            if (doctorEntity == null)
                throw new Exception("Doctor not found");

            doctorEntity.Salary = dto.Salary;
            doctorEntity.WorkingHours = dto.WorkingHours;
            doctorEntity.SpecialtyID = dto.SpecialityID;
            doctorEntity.ImageUrl = dto.ImageUrl;
            doctorEntity.ConsultationFee = dto.ConsultationFee;
            doctorEntity.YearsOfExperience = dto.YearsOfExperience;

            if (!string.IsNullOrEmpty(dto.DisplayName))
            {
                doctorEntity.User.DisplayName = dto.DisplayName.StartsWith("Dr.")
                    ? dto.DisplayName
                    : $"Dr. {dto.DisplayName}";
            }

            await _unitOfWork.Repository<Doctor>().UpdateAsync(doctorEntity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteDoctorAsync(int id)
        {
            var doctor = await _unitOfWork.Repository<Doctor>().GetByIdAsync(id);

            if (doctor == null)
                throw new Exception("Doctor not found");

            await _unitOfWork.Repository<Doctor>().DeleteAsync(doctor);
            await _unitOfWork.CompleteAsync();
        }

        public async Task ToggleDoctorStatusAsync(int id)
        {
            var doctor = await _unitOfWork.Repository<Doctor>().GetByIdAsync(id);

            if (doctor == null)
                throw new Exception("Doctor not found");

            var user = await _userManager.FindByIdAsync(doctor.UserId);

            if (user.LockoutEnd == null)
                user.LockoutEnd = DateTimeOffset.MaxValue;
            else
                user.LockoutEnd = null;

            await _userManager.UpdateAsync(user);
        }
    }
}