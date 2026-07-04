using SmartTeethCare.Core.DTOs.DoctorModule;
using SmartTeethCare.Core.DTOs.PatientModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.Interfaces.Services
{
    public interface IPrescriptionService
    {
        Task CreatePrescriptionAsync(CreatePrescriptionDto dto, string doctorUserId);

        Task<List<PrescriptionDetailsDTO>> GetPrescriptionsByPatientIdAsync(int patientId, ClaimsPrincipal user);
    }
}

