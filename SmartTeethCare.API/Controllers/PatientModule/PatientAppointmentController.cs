using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTeethCare.Core.DTOs.PatientModule;
using SmartTeethCare.Core.Interfaces.Services.PatientModule;
using System.Security.Claims;

namespace SmartTeethCare.Web.Areas.Patient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Patient")]
    public class PatientAppointmentController : ControllerBase
    {
        private readonly IPatientAppointmentService _appointmentService;

        public PatientAppointmentController(IPatientAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpPost(nameof(BookAppointment))]
        public async Task<IActionResult> BookAppointment([FromBody] BookAppointmentDto dto)
        {
            try
            {
                await _appointmentService.BookAppointment(dto, User); 
                return Ok(new { message = "Appointment booked successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPatch("CancelAppointment/{appointmentId}")]
        public async Task<IActionResult> CancelAppointment(int appointmentId)
        {
            try
            {
                await _appointmentService.CancelAppointment(appointmentId, User);
                return Ok(new { message = "Appointment canceled successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet(nameof(GetMyAppointments))]
        public async Task<IActionResult> GetMyAppointments()
        {
            try
            {
                var appointments = await _appointmentService.GetMyAppointments(User);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("GetAppointmentDetails/{appointmentId}")]
        public async Task<IActionResult> GetAppointmentDetails(int appointmentId)
        {
            var result = await _appointmentService
                .GetAppointmentDetails(appointmentId, User);

            return Ok(result);
        }
    }
}
