using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SmartTeethCare.Core.DTOs.DoctorModule;
using SmartTeethCare.Core.DTOs.PatientModule;

namespace SmartTeethCare.Core.Interfaces.Services
{
    public interface IPrescriptionService
    {
        Task CreatePrescriptionAsync(CreatePrescriptionDto dto, string doctorUserId);

        Task<List<PrescriptionDetailsDTO>>
            GetPrescriptionsByPatientIdAsync(int patientId);
    }
}

