using SmartTeethCare.Core.DTOs.DoctorModule;

namespace SmartTeethCare.Core.Interfaces.Services.DoctorModule
{
    public interface IAppointmentBookingService
    {
        Task<BookAppointmentResultDto> BookAppointmentAsync(BookAppointmentDto dto);
    }
}