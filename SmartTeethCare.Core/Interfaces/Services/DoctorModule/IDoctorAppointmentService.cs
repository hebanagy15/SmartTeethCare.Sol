using SmartTeethCare.Core.DTOs.DoctorModule;
using SmartTeethCare.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.Interfaces.Services.DoctorModule
{

    public interface IDoctorAppointmentService
    {
        Task<List<DoctorAppointmentDto>> GetDoctorAppointmentsAsync(
            string userId,
            AppointmentStatus? status,
            DateTime? date,
            string? search);

        Task CompleteAppointmentAsync(int appointmentId, string userId);

        Task CancelAppointmentAsync(int appointmentId, string userId);

        Task<DoctorDashboardDto> GetDoctorDashboardAsync(string userId);
    }

}
