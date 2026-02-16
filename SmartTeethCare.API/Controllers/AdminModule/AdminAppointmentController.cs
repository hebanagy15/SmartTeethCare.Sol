using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTeethCare.Core.DTOs.AdminModule;
using SmartTeethCare.Core.Enums;
using SmartTeethCare.Core.Interfaces.Services.AdminModule;

[ApiController]
[Route("api/admin/appointments")]
[Authorize(Roles = "Admin")]
public class AdminAppointmentController : ControllerBase
{
    private readonly IAdminAppointmentService _appointmentService;

    public AdminAppointmentController(IAdminAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentByAdminDTO dto)
        => Ok(await _appointmentService.CreateAppointmentByAdminAsync(dto));

    [HttpGet]
    public async Task<IActionResult> GetAllAppointments()
        => Ok(await _appointmentService.GetAllAppointmentsAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAppointmentById(int id)
    {
        var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
        if (appointment == null) return NotFound();
        return Ok(appointment);
    }

    [HttpPatch("{id}/cancel")]
    public async Task<IActionResult> CancelAppointment(int id)
    {
        await _appointmentService.CancelAppointmentAsync(id);
        return Ok();
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> ChangeStatus(int id, [FromBody] AppointmentStatus status)
    {
        await _appointmentService.ChangeAppointmentStatusAsync(id, status);
        return Ok();
    }
}
