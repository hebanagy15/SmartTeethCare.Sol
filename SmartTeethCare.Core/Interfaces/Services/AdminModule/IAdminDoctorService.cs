using SmartTeethCare.Core.DTOs.AdminModule;
using SmartTeethCare.Core.DTOs.MedicalRecordModule;
using SmartTeethCare.Core.DTOs.PatientModule;
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
        Task ToggleDoctorStatusAsync(int id, bool cancelAppointments = false);
        Task<IEnumerable<MedicalRecordDto>> GetDoctorMedicalRecordsAsync(int doctorId);
        Task<IEnumerable<PrescriptionDetailsDTO>> GetDoctorPrescriptionsAsync(int doctorId);
    }

}
