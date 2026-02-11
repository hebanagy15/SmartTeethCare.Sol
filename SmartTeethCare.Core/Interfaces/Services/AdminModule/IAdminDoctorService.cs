using SmartTeethCare.Core.DTOs.AdminModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.Interfaces.Services.AdminModule
{
    public interface IAdminDoctorService
    {
        Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync();
        Task<DoctorDto?> GetDoctorByIdAsync(int id);
        Task AddDoctorAsync(CreateDoctorDto dto);
        Task UpdateDoctorAsync(int id, UpdateDoctorDto dto);
        Task DeleteDoctorAsync(int id);
        Task ToggleDoctorStatusAsync(int id);
    }

}
