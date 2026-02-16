using SmartTeethCare.Core.DTOs.AdminModule;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Interfaces.Services.AdminModule;
using SmartTeethCare.Core.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Service.AdminModule
{
    public class AdminSpecialityService : IAdminSpecialityService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminSpecialityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<SpecialityDTO>> GetAllAsync()
        {
            var specialities = await _unitOfWork.Repository<Speciality>().GetAllAsync();
            return specialities.Select(s => new SpecialityDTO
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description
            });
        }
        public async Task<SpecialityDTO> GetByIdAsync(int id)
        {
            var speciality = await _unitOfWork.Repository<Speciality>().GetByIdAsync(id);
            if (speciality == null)
                throw new KeyNotFoundException($"Speciality with Id {id} not found");

            return new SpecialityDTO
            {
                Id = speciality.Id,
                Name = speciality.Name,
                Description = speciality.Description
            };
        }

        public async Task<SpecialityDTO> CreateAsync(SpecialityDTO dto, ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)?? throw new Exception("User not authenticated");
            var Admins = await _unitOfWork.Repository<Admin>()
                .FindAsync(p => p.UserId == userId);

            var Admin = Admins.FirstOrDefault();

            if (Admin == null)
                throw new Exception("Admin not found");
            var AdminId = Admin.Id;

            var speciality = new Speciality
            {
                Name = dto.Name,
                Description = dto.Description
            };

            await _unitOfWork.Repository<Speciality>().AddAsync(speciality);
            await _unitOfWork.CompleteAsync();

            dto.Id = speciality.Id;
            return dto;
        }
        public async Task<bool> UpdateAsync(SpecialityDTO dto, ClaimsPrincipal user)
        {
            if (!dto.Id.HasValue)
                throw new ArgumentException("Id is required for update");

            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)?? throw new Exception("User not authenticated");
            var Admins = await _unitOfWork.Repository<Admin>()
                .FindAsync(p => p.UserId == userId);

            var Admin = Admins.FirstOrDefault();

            if (Admin == null)
                throw new Exception("Admin not found");
            var AdminId = Admin.Id;

            var repository = _unitOfWork.Repository<Speciality>();
            var speciality = await repository.GetByIdAsync(dto.Id.Value);
            if (speciality == null) return false;

            speciality.Name = dto.Name;
            speciality.Description = dto.Description;

            await repository.UpdateAsync(speciality);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id, ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)?? throw new Exception("User not authenticated");
            var Admins = await _unitOfWork.Repository<Admin>()
                .FindAsync(p => p.UserId == userId);

            var Admin = Admins.FirstOrDefault();

            if (Admin == null)
                throw new Exception("Admin not found");
            var AdminId = Admin.Id;

            var repository = _unitOfWork.Repository<Speciality>();
            var speciality = await repository.GetByIdAsync(id);
            if (speciality == null)
                throw new KeyNotFoundException($"Speciality with Id {id} not found");

            await repository.DeleteAsync(speciality);
            await _unitOfWork.CompleteAsync();
            return true;
        }


    }
}
