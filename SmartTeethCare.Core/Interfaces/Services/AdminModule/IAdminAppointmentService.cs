using SmartTeethCare.Core.DTOs.AdminModule;
using SmartTeethCare.Core.DTOs.PatientModule;
using SmartTeethCare.Core.Enums;

namespace SmartTeethCare.Core.Interfaces.Services.AdminModule
{
    public interface IAdminAppointmentService
    {
        Task<AppointmentResponseDTO> CreateAppointmentByAdminAsync(CreateAppointmentByAdminDTO dto);
        Task<IEnumerable<AppointmentDetailsDTO>> GetAllAppointmentsAsync();
        Task<AppointmentDetailsDTO?> GetAppointmentByIdAsync(int id);
        Task CancelAppointmentAsync(int id);
        Task ChangeAppointmentStatusAsync(int id, AppointmentStatus status);
    }
}

