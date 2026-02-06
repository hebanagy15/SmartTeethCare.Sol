using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTeethCare.Core.Enums;
using SmartTeethCare.Core.Interfaces.Services.DoctorModule;
using System.Security.Claims;


namespace SmartTeethCare.API.Controllers.DoctorModule
{
    [Authorize(Roles = "Doctor")]
    [ApiController]
    [Route("api/doctor/appointments")]
    public class DoctorAppointmentsController : ControllerBase
    {
        private readonly IDoctorAppointmentService _service;

        public DoctorAppointmentsController(IDoctorAppointmentService service)
        {
            _service = service;
        }

        private string UserId =>
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        [HttpGet]
        public async Task<IActionResult> GetAppointments(
            AppointmentStatus? status,
            DateTime? date,
            string? search)
        {
            return Ok(await _service.GetDoctorAppointmentsAsync(
                UserId, status, date, search));
        }

        [HttpPut("{id}/complete")]
        public async Task<IActionResult> Complete(int id)
        {
            await _service.CompleteAppointmentAsync(id, UserId);
            return Ok();
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            await _service.CancelAppointmentAsync(id, UserId);
            return Ok();
        }

        [HttpGet("/api/doctor/dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            return Ok(await _service.GetDoctorDashboardAsync(UserId));
        }
    }
}