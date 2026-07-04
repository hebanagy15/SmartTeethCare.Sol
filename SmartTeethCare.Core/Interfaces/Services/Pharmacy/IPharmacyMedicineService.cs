using SmartTeethCare.Core.DTOs.Pharmacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.Interfaces.Services.Pharmacy
{
    public interface IPharmacyMedicineService
    {
        Task<IEnumerable<PharmacyMedicineDto>> GetAllAsync();

        Task<PharmacyMedicineDto> GetByIdAsync(int id);

        Task AddAsync(CreatePharmacyMedicineDto dto);

        Task UpdateAsync(UpdatePharmacyMedicineDto dto);

        Task DeleteAsync(int pharmacyId, int medicineId);

        Task<IEnumerable<PharmacyMedicineDto>> GetMedicinesByPharmacyAsync(int pharmacyId);
    }
}
