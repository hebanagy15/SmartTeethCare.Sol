using Microsoft.EntityFrameworkCore;
using SmartTeethCare.Core.DTOs.DoctorModule;
using SmartTeethCare.Core.DTOs.PatientModule;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Interfaces.Services;
using SmartTeethCare.Core.Interfaces.UnitOfWork;

namespace SmartTeethCare.Service.Implementation
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PrescriptionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreatePrescriptionAsync(CreatePrescriptionDto dto, string doctorUserId)
        {
            // 1️⃣ Get Appointment
            var appointment = await _unitOfWork
                .Repository<Appointment>()
                .GetByIdAsync(dto.AppointmentId);

            if (appointment == null)
                throw new Exception("Appointment not found");

            // 2️⃣ Get Doctor by UserId
            var doctor = (await _unitOfWork
                .Repository<Doctor>()
                .FindAsync(d => d.UserId == doctorUserId))
                .FirstOrDefault();

            if (doctor == null)
                throw new Exception("Doctor not found");

            // 3️⃣ Validate ownership
            if (appointment.DoctorID != doctor.Id)
                throw new Exception("Unauthorized access");

            // 4️⃣ Create Prescription
            var prescription = new Prescription
            {
                AppointmentId = appointment.Id,
                DoctorId = doctor.Id,
                PatientId = appointment.PatientID,
                Date = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Prescription>().AddAsync(prescription);

            foreach (var med in dto.Medicines)
            {
                var prescriptionMedicine = new PrescriptionMedicine
                {
                    PrescriptionID = prescription.Id,
                    MedicineID = med.MedicineId,
                    Dosage = med.Dosage,
                    Frequency = med.Frequency,
                    DurationInDays = med.DurationInDays,
                    Quantity = med.Quantity,
                    Instructions = med.Instructions
                };

                await _unitOfWork
                    .Repository<PrescriptionMedicine>()
                    .AddAsync(prescriptionMedicine);
            }



        }


        public async Task<List<PrescriptionDetailsDTO>>
            GetPrescriptionsByPatientIdAsync(int patientId)
        {
            var prescriptions = await _unitOfWork
                .Repository<Prescription>()
                .FindAsync(
                    p => p.PatientId == patientId,
                    q => q
                        .Include(p => p.doctor).ThenInclude(d => d.User)
                        .Include(p => p.Patient).ThenInclude(p => p.User)
                        .Include(p => p.PrescriptionMedicines)
                            .ThenInclude(pm => pm.Medicine)
                );

            return prescriptions.Select(p => new PrescriptionDetailsDTO
            {
                PrescriptionId = p.Id,
                Date = p.Date,
                DoctorName = p.doctor.User.UserName,
                PatientName = p.Patient.User.UserName,
                Medicines = p.PrescriptionMedicines
        .Select(pm => new PrescriptionMedicineDetailsDto
        {
            MedicineName = pm.Medicine.Name,
            Dosage = pm.Dosage,
            Frequency = pm.Frequency,
            DurationInDays = pm.DurationInDays,
            Quantity = pm.Quantity,
            Instructions = pm.Instructions
        })
        .ToList()
            }).ToList();
        }
    }
}
