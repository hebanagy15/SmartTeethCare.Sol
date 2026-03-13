using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTeethCare.Core.Interfaces.Services.DoctorModule;
using System.Security.Claims;

namespace SmartTeethCare.API.Controllers.DoctorModule
{
    [Authorize(Roles = "Doctor")]
    [ApiController]
    [Route("api/doctor/dashboard")]
    public class DoctorDashboardController : ControllerBase
    {
        private readonly IDoctorAppointmentService _service;

        public DoctorDashboardController(IDoctorAppointmentService service)
        {
            _service = service;
        }

        private string UserId =>
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            var dashboard = await _service.GetDoctorDashboardAsync(UserId);
            return Ok(dashboard);
        }
    }
}