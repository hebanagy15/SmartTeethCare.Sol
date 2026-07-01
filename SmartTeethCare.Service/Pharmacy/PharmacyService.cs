using SmartTeethCare.Core.DTOs.Pharmacy;
using SmartTeethCare.Core.Interfaces.Services.Pharmacy;
using SmartTeethCare.Core.Interfaces.UnitOfWork;
using PharmacyEntity = SmartTeethCare.Core.Entities.Pharmacy;

namespace SmartTeethCare.Service.Pharmacy
{
    public class PharmacyService : IPharmacyService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PharmacyService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<PharmacyDto>> GetAllAsync()
        {
            var pharmacies = await _unitOfWork.Repository<PharmacyEntity>().GetAllAsync();
            return pharmacies.Select(p => new PharmacyDto { Id = p.Id, Name = p.Name, Address = p.Address, Phone = p.Phone, WorkingHours = p.WorkingHours });
        }
        public async Task<PharmacyDetailsDto> GetByIdAsync(int id)
        {
            var pharmacy = await _unitOfWork.Repository<PharmacyEntity>().GetByIdAsync(id);
            if (pharmacy == null)
                throw new Exception("Pharmacy not found.");
            return new PharmacyDetailsDto
            {
                Id = pharmacy.Id,
                Name = pharmacy.Name,
                Address = pharmacy.Address,
                Phone = pharmacy.Phone,
                WorkingHours = pharmacy.WorkingHours,
                Latitude = pharmacy.Latitude,
                Longitude = pharmacy.Longitude
            };
        }
        public async Task AddAsync(CreatePharmacyDto dto)
        {
            var pharmacy = new PharmacyEntity
            {
                Name = dto.Name,
                Address = dto.Address,
                Phone = dto.Phone,
                WorkingHours = dto.WorkingHours,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude
            };
            await _unitOfWork.Repository<PharmacyEntity>().AddAsync(pharmacy);
            await _unitOfWork.CompleteAsync();
        }
        public async Task UpdateAsync(UpdatePharmacyDto dto)
        {
            var pharmacy = await _unitOfWork.Repository<PharmacyEntity>().GetByIdAsync(dto.Id);
            if (pharmacy == null)
                throw new Exception("Pharmacy not found.");
            pharmacy.Name = dto.Name;
            pharmacy.Address = dto.Address;
            pharmacy.Phone = dto.Phone;
            pharmacy.WorkingHours = dto.WorkingHours;
            pharmacy.Latitude = dto.Latitude;
            pharmacy.Longitude = dto.Longitude;

            await _unitOfWork.Repository<PharmacyEntity>().UpdateAsync(pharmacy);
            await _unitOfWork.CompleteAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var pharmacy = await _unitOfWork.Repository<PharmacyEntity>().GetByIdAsync(id);
            if (pharmacy == null)
                throw new Exception("Pharmacy not found.");
            await _unitOfWork.Repository<PharmacyEntity>().DeleteAsync(pharmacy);
            await _unitOfWork.CompleteAsync();
        }
    }

}
