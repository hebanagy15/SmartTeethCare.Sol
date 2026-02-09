using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTeethCare.Core.DTOs.AdminModule;
using SmartTeethCare.Core.Interfaces.Services.AdminModule;

namespace SmartTeethCare.API.Controllers.AdminModule
{
  
        [ApiController]
        [Route("api/admin/CreateAnAppointment")]
        [Authorize(Roles = "Admin")]
        public class AdminAppointmentController : ControllerBase
        {
            private readonly IAdminAppointmentService _appointmentService;

            public AdminAppointmentController(IAdminAppointmentService appointmentService)
            {
                _appointmentService = appointmentService;
            }

            [HttpPost]
            public async Task<IActionResult> CreateAppointmentForPatient([FromBody] CreateAppointmentByAdminDTO dto)
            {
                var result = await _appointmentService.CreateAppointmentByAdminAsync(dto);
                return Ok(result);
            }
        }
    

}
