using SmartTeethCare.Core.DTOs.PatientModule;
using SmartTeethCare.Core.Entities;
using System.Security.Claims;

namespace SmartTeethCare.Core.Interfaces.Services.PatientModule
{
    public interface IPatientAppointmentService
    {
        Task BookAppointment(BookAppointmentDto dto, ClaimsPrincipal user);
        Task<List<Appointment>> GetMyAppointments(ClaimsPrincipal user);
        Task CancelAppointment(int appointmentId, ClaimsPrincipal user);
        Task<AppointmentDetailsDTO> GetAppointmentDetails(int appointmentId, ClaimsPrincipal user);
    }
}
