using SmartTeethCare.Core.DTOs.PatientModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.Interfaces.Services.PatientModule
{
    public interface IPatientPrescriptionService
    {
        Task<List<PrescriptionDetailsDTO>> GetMyPrescriptionsAsync(ClaimsPrincipal user);
        Task<PrescriptionDetailsDTO> GetByAppointmentAsync(int appointmentId, ClaimsPrincipal user);
    }

}
