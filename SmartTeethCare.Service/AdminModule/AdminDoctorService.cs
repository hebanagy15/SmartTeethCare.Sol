using Microsoft.AspNetCore.Identity;
using SmartTeethCare.Core.DTOs.AdminModule;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Interfaces.Services.AdminModule;
using SmartTeethCare.Core.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                FullName = d.User.UserName,
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
                FullName = doctor.User.UserName,
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
                UserName = dto.FullName,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new Exception("User creation failed");

            await _userManager.AddToRoleAsync(user, "Doctor");

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
            var doctor = await _unitOfWork.Repository<Doctor>().GetByIdAsync(id);
            if (doctor == null)
                throw new Exception("Doctor not found");

            doctor.Salary = dto.Salary;
            doctor.WorkingHours = dto.WorkingHours;
            doctor.SpecialtyID = dto.SpecialityID;

            await _unitOfWork.Repository<Doctor>().UpdateAsync(doctor);
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
                user.LockoutEnd = DateTimeOffset.MaxValue; // deactivate
            else
                user.LockoutEnd = null; // activate

            await _userManager.UpdateAsync(user);
        }
    }

}
