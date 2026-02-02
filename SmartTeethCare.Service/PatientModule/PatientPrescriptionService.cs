using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartTeethCare.Core.DTOs.PatientModule;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Interfaces.Services.PatientModule;
using SmartTeethCare.Core.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Service.PatientModule
{
    public class PatientPrescriptionService : IPatientPrescriptionService
    {
        private readonly IUnitOfWork _uow;
        private readonly UserManager<User> _userManager;

        public PatientPrescriptionService(IUnitOfWork uow, UserManager<User> userManager)
        {
            _uow = uow;
            _userManager = userManager;
        }
        public async Task<List<PrescriptionDetailsDTO>> GetMyPrescriptionsAsync(ClaimsPrincipal user)
        {
            var patientId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));

            var prescriptionRepo = _uow.Repository<Prescription>();

            var prescriptions = await prescriptionRepo.FindAsync(
                p => p.PatientId == patientId,
                q => q.Include(p => p.doctor)
                      .ThenInclude(d => d.User)
                      .Include(p => p.Patient)
                      .Include(p => p.PrescriptionMedicines)
                      .ThenInclude(pm => pm.Medicine)
            );

            return prescriptions.Select(p => new PrescriptionDetailsDTO
            {
                PrescriptionId = p.Id,
                Date = p.Date,
                DoctorName = p.doctor?.User?.UserName ?? "Unknown Doctor",
                PatientName = p.Patient?.User?.UserName ?? "Unknown Patient",
                Medicines = p.PrescriptionMedicines
                    .Select(pm => pm.Medicine.Name)
                    .ToList(),

            }).ToList();
        }

        public async Task<PrescriptionDetailsDTO> GetByAppointmentAsync(int appointmentId, ClaimsPrincipal user)
        {
            var patientId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));

            var prescriptionRepo = _uow.Repository<Prescription>();

            var prescription = (await prescriptionRepo.FindAsync(
                p => p.AppointmentId == appointmentId,
                q => q.Include(p => p.doctor)
                      .ThenInclude(d => d.User)
                      .Include(p => p.PrescriptionMedicines)
                      .ThenInclude(pm => pm.Medicine)
            )).FirstOrDefault();

            if (prescription == null || prescription.PatientId != patientId)
                throw new UnauthorizedAccessException();

            return new PrescriptionDetailsDTO
            {
                PrescriptionId = prescription.Id,
                Date = prescription.Date,
                DoctorName = prescription.doctor.User.UserName,
                Medicines = prescription.PrescriptionMedicines
                    .Select(pm => pm.Medicine.Name)
                    .ToList()
            };
        }


    }
}
