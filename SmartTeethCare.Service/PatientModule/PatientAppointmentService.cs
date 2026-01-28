using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartTeethCare.Core.DTOs.PatientModule;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Interfaces.Services.PatientModule;
using SmartTeethCare.Core.Interfaces.UnitOfWork;
using System;
using System.Security.Claims;

namespace SmartTeethCare.Service.PatientModule
{
    public class PatientAppointmentService : IPatientAppointmentService
    {
        private readonly IUnitOfWork _uow;
        private readonly UserManager<User> _userManager;

        public PatientAppointmentService(IUnitOfWork uow , UserManager<User> userManager)
        {
            _uow = uow;
            _userManager = userManager;
        }

        public async Task BookAppointment(BookAppointmentDto dto, ClaimsPrincipal user)
        {
            // user = Token
            var patientId = int.Parse(
                user.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );
            // Avoid Duplication
            var isAlreadyBooked = await _uow.Repository<Appointment>()
            .AnyAsync(a => a.PatientID == patientId && a.DoctorID == dto.DentistId && a.Date == dto.AppointmentDate);

            if (isAlreadyBooked)
                throw new Exception("You already booked this appointment");
            
            var doctorBusy = await _uow.Repository<Appointment>().AnyAsync(a => a.DoctorID == dto.DentistId && a.Date == dto.AppointmentDate && a.Status != "Cancelled"
    );

            if (doctorBusy)
                throw new Exception("Doctor is not available at this time");



            var appointment = new Appointment
            {
                PatientID = patientId,
                DoctorID = dto.DentistId,
                Date = dto.AppointmentDate,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            
            await _uow.Repository<Appointment>().AddAsync(appointment);
            await _uow.CompleteAsync();

        }

        public async Task CancelAppointment(int appointmentId, ClaimsPrincipal user)
        {
            var patientId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));

            var appointment = await _uow.Repository<Appointment>()
                .GetAllAsync(); 
            var target = appointment.FirstOrDefault(a => a.Id == appointmentId && a.PatientID == patientId);

            if (target == null)
                throw new Exception("Appointment not found or not yours.");

            
            if (target.Date <= DateTime.Now.AddHours(1))
                throw new Exception("Cannot cancel appointment less than 1 hour before it starts.");

            target.Status = "Canceled";
            _uow.Repository<Appointment>().UpdateAsync(target);
            await _uow.CompleteAsync();
        }

        public async Task<List<Appointment>> GetMyAppointments(ClaimsPrincipal user)
        {
            
            var patientId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));

            
            var appointments = await _uow.Repository<Appointment>()
                .FindAsync(a => a.PatientID == patientId);

            
            var orderedAppointments = appointments
                .OrderBy(a => a.Date)
                .ToList();

            
            return orderedAppointments;
        }

        public async Task<AppointmentDetailsDTO> GetAppointmentDetails(int appointmentId, ClaimsPrincipal user)
        {
            
            var patientId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));

            
            var appointment = (await _uow.Repository<Appointment>()
                .FindAsync(a => a.Id == appointmentId))
                .FirstOrDefault();

            if (appointment == null)
                throw new Exception("Appointment not found");

            
            var doctor = await _uow.Repository<Doctor>().GetByIdAsync(appointment.DoctorID);
            var patient = await _uow.Repository<Patient>().GetByIdAsync(appointment.PatientID);

            var doctorUser = await _userManager.FindByIdAsync(doctor.UserId);
            var patientUser = await _userManager.FindByIdAsync(patient.UserId);

            var doctorName = doctorUser?.UserName;
            var patientName = patientUser?.UserName;


            var dto = new AppointmentDetailsDTO
            {
                Id = appointment.Id,
                DoctorId = appointment.DoctorID,
                DoctorName = doctorName ,
                PatientId = appointment.PatientID,
                PatientName = patientName,
                Date = appointment.Date,
                Status = appointment.Status,
                CreatedAt = appointment.CreatedAt,
                Amount = appointment.Amount
            };

            return dto;
        }


    }

}


