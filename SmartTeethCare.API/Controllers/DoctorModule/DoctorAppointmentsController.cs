using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Enums;
using SmartTeethCare.Repository;
using System.Security.Claims;
using SmartTeethCare.Repository.Data;

namespace SmartTeethCare.API.Controllers.DoctorModule
{
    [ApiController]
    [Route("api/doctor/[controller]")]
    [Authorize(Roles = "Doctor")]
    public class DoctorAppointmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DoctorAppointmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Helper method to get current doctor Id
        private async Task<int> GetCurrentDoctorId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);

            if (doctor == null)
                throw new UnauthorizedAccessException("Doctor not found");

            return doctor.Id;
        }

        // ✅ Get all appointments (optional filter by status)
        [HttpGet]
        public async Task<IActionResult> GetAppointments([FromQuery] AppointmentStatus? status = null)
        {
            var doctorId = await GetCurrentDoctorId();
            var query = _context.Appointments
                .Include(a => a.patient)
                .Where(a => a.DoctorID == doctorId)
                .AsQueryable();

            if (status != null)
                query = query.Where(a => a.Status == status);

            var appointments = await query.ToListAsync();
            return Ok(appointments);
        }

        // ✅ Approve Appointment
        [HttpPost("{appointmentId}/approve")]
        public async Task<IActionResult> ApproveAppointment(int appointmentId)
        {
            var doctorId = await GetCurrentDoctorId();
            var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == appointmentId && a.DoctorID == doctorId);

            if (appointment == null)
                return NotFound("Appointment not found or you are not authorized");

            if (appointment.Status != AppointmentStatus.Pending)
                return BadRequest("Only Pending appointments can be approved");

            appointment.Status = AppointmentStatus.Approved;
            await _context.SaveChangesAsync();
            return Ok(appointment);
        }

        // ✅ Reject Appointment
        [HttpPost("{appointmentId}/reject")]
        public async Task<IActionResult> RejectAppointment(int appointmentId)
        {
            var doctorId = await GetCurrentDoctorId();
            var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == appointmentId && a.DoctorID == doctorId);

            if (appointment == null)
                return NotFound("Appointment not found or you are not authorized");

            if (appointment.Status != AppointmentStatus.Pending)
                return BadRequest("Only Pending appointments can be rejected");

            appointment.Status = AppointmentStatus.Rejected;
            await _context.SaveChangesAsync();
            return Ok(appointment);
        }

        // ✅ Complete Appointment
        [HttpPost("{appointmentId}/complete")]
        public async Task<IActionResult> CompleteAppointment(int appointmentId)
        {
            var doctorId = await GetCurrentDoctorId();
            var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == appointmentId && a.DoctorID == doctorId);

            if (appointment == null)
                return NotFound("Appointment not found or you are not authorized");

            if (appointment.Status != AppointmentStatus.Pending)
                return BadRequest("Only Pending appointments can be marked as Completed");

            appointment.Status = AppointmentStatus.Completed;
            await _context.SaveChangesAsync();
            return Ok(appointment);
        }

        // ✅ View Patient Profile (any status)
        [HttpGet("{appointmentId}/patient-profile")]
        public async Task<IActionResult> ViewPatientProfile(int appointmentId)
        {
            var doctorId = await GetCurrentDoctorId();
            var appointment = await _context.Appointments
                .Include(a => a.patient)
                .FirstOrDefaultAsync(a => a.Id == appointmentId && a.DoctorID == doctorId);

            if (appointment == null)
                return NotFound("Appointment not found or you are not authorized");

            return Ok(appointment.patient);
        }

        // ✅ Add Prescription (only Pending)
        [HttpPost("{appointmentId}/prescription")]
        public async Task<IActionResult> AddPrescription(int appointmentId, [FromBody] Prescription prescription)
        {
            var doctorId = await GetCurrentDoctorId();
            var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == appointmentId && a.DoctorID == doctorId);

            if (appointment == null)
                return NotFound("Appointment not found or you are not authorized");

            if (appointment.Status != AppointmentStatus.Pending)
                return BadRequest("You can only add prescriptions for Pending appointments");

            prescription.DoctorId = doctorId;
            prescription.PatientId = appointment.PatientID;
            prescription.AppointmentId = appointment.Id;

            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();
            return Ok(prescription);
        }

        // ✅ View Reports / Prescriptions (only Completed)
        [HttpGet("{appointmentId}/report")]
        public async Task<IActionResult> ViewReport(int appointmentId)
        {
            var doctorId = await GetCurrentDoctorId();
            var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == appointmentId && a.DoctorID == doctorId);

            if (appointment == null)
                return NotFound("Appointment not found or you are not authorized");

            if (appointment.Status != AppointmentStatus.Completed)
                return BadRequest("Reports are only available for Completed appointments");

            var prescription = await _context.Prescriptions.Where(p => p.AppointmentId == appointment.Id).ToListAsync();
            return Ok(new { Appointment = appointment, Prescriptions = prescription });
        }

        // ✅ Get Today's Appointments
        [HttpGet("today")]
        public async Task<IActionResult> GetTodayAppointments()
        {
            var doctorId = await GetCurrentDoctorId();
            var today = DateTime.Today;

            var appointments = await _context.Appointments
                .Where(a => a.DoctorID == doctorId && a.Date.Date == today)
                .Include(a => a.patient)
                .ToListAsync();

            return Ok(appointments);
        }
    }
}
