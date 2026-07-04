using SmartTeethCare.Core.DTOs.Pharmacy;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Interfaces.Services.Pharmacy;
using SmartTeethCare.Core.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
namespace SmartTeethCare.Service.Pharmacy
{
    public class PharmacyMedicineService : IPharmacyMedicineService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PharmacyMedicineService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PharmacyMedicineDto>> GetAllAsync()
        {
            var pharmacyMedicines = await _unitOfWork
                .Repository<PharmacyMedicine>()
                .FindAsync(include: q => q
                    .Include(pm => pm.Pharmacy)
                    .Include(pm => pm.Medicine));

            return pharmacyMedicines.Select(pm => new PharmacyMedicineDto
            {
                Id = pm.Id,
                PharmacyID = pm.PharmacyID,
                PharmacyName = pm.Pharmacy.Name,
                MedicineID = pm.MedicineID,
                MedicineName = pm.Medicine.Name,
                Price = pm.Medicine.Price,
                StockQuantity = pm.StockQuantity
            });
        }

        public async Task<PharmacyMedicineDto> GetByIdAsync(int id)
        {
            var pharmacyMedicine = (await _unitOfWork
                .Repository<PharmacyMedicine>()
                .FindAsync(
                    pm => pm.Id == id,
                    include: q => q
                        .Include(pm => pm.Pharmacy)
                        .Include(pm => pm.Medicine)))
                .FirstOrDefault();

            if (pharmacyMedicine == null)
                throw new Exception("Pharmacy medicine not found.");

            return new PharmacyMedicineDto
            {
                Id = pharmacyMedicine.Id,
                PharmacyID = pharmacyMedicine.PharmacyID,
                PharmacyName = pharmacyMedicine.Pharmacy.Name,
                MedicineID = pharmacyMedicine.MedicineID,
                MedicineName = pharmacyMedicine.Medicine.Name,
                Price = pharmacyMedicine.Medicine.Price,
                StockQuantity = pharmacyMedicine.StockQuantity
            };
        }

        public async Task AddAsync(CreatePharmacyMedicineDto dto)
        {
            var pharmacyExists = await _unitOfWork
                .Repository<Core.Entities.Pharmacy>()
                .AnyAsync(p => p.Id == dto.PharmacyID);

            if (!pharmacyExists)
                throw new Exception("Pharmacy not found.");

            var medicineExists = await _unitOfWork
                .Repository<Medicine>()
                .AnyAsync(m => m.Id == dto.MedicineID);

            if (!medicineExists)
                throw new Exception("Medicine not found.");

            var alreadyExists = await _unitOfWork
                .Repository<PharmacyMedicine>()
                .AnyAsync(pm =>
                    pm.PharmacyID == dto.PharmacyID &&
                    pm.MedicineID == dto.MedicineID);

            if (alreadyExists)
                throw new Exception("This medicine already exists in this pharmacy.");

            var pharmacyMedicine = new PharmacyMedicine
            {
                PharmacyID = dto.PharmacyID,
                MedicineID = dto.MedicineID,
                StockQuantity = dto.StockQuantity
            };

            await _unitOfWork.Repository<PharmacyMedicine>().AddAsync(pharmacyMedicine);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateAsync(UpdatePharmacyMedicineDto dto)
        {
            var pharmacyMedicine = (await _unitOfWork
     .Repository<PharmacyMedicine>()
     .FindAsync(
         pm => pm.PharmacyID == dto.PharmacyID &&
               pm.MedicineID == dto.MedicineID))
     .FirstOrDefault();

            if (pharmacyMedicine == null)
                throw new Exception("Pharmacy medicine not found.");

            pharmacyMedicine.StockQuantity = dto.StockQuantity;

            await _unitOfWork.Repository<PharmacyMedicine>().UpdateAsync(pharmacyMedicine);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int pharmacyId, int medicineId)
        {
            var pharmacyMedicine = (await _unitOfWork
                .Repository<PharmacyMedicine>()
                .FindAsync(pm =>
                    pm.PharmacyID == pharmacyId &&
                    pm.MedicineID == medicineId))
                .FirstOrDefault();

            if (pharmacyMedicine == null)
                throw new Exception("Pharmacy medicine not found.");

            await _unitOfWork.Repository<PharmacyMedicine>().DeleteAsync(pharmacyMedicine);
            await _unitOfWork.CompleteAsync();
        }


    }

    }
