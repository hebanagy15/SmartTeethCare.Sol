using SmartTeethCare.Core.DTOs.AdminModule;

namespace SmartTeethCare.Core.Interfaces.Services.AdminModule
{
    public interface IAdminAppointmentService
    {
        Task<AppointmentResponseDTO> CreateAppointmentByAdminAsync(CreateAppointmentByAdminDTO dto);
    }
}

