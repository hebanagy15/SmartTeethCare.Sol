using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTeethCare.API.Errors;
using SmartTeethCare.Core.DTOs.DoctorModule;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Interfaces.Services.DoctorModule;
using SmartTeethCare.Core.Interfaces.UnitOfWork;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class DoctorScheduleController : ControllerBase
{
    private readonly IDoctorScheduleService _scheduleService;
    private readonly IUnitOfWork _uow;

    public DoctorScheduleController(IDoctorScheduleService scheduleService, IUnitOfWork uow)
    {
        _scheduleService = scheduleService;
        _uow = uow;
    }

    // GET api/DoctorSchedule?doctorId=5
    [HttpGet]
    public async Task<IActionResult> GetDoctorSchedule([FromQuery] int? doctorId = null)
    {
        int resolvedDoctorId;

        if (doctorId.HasValue && doctorId.Value > 0)
        {
            // Called by patient/booking — use the supplied ID directly
            resolvedDoctorId = doctorId.Value;
        }
        else
        {
            // Called by the logged-in doctor — resolve from JWT
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new ApiResponse(401, "User not authenticated"));

            var doctors = await _uow.Repository<Doctor>().FindAsync(d => d.UserId == userId);
            var doctor = doctors.FirstOrDefault();
            if (doctor == null)
                return NotFound(new ApiResponse(404, "Doctor not found"));

            resolvedDoctorId = doctor.Id;
        }

        var schedule = await _scheduleService.GetDoctorScheduleAsync(resolvedDoctorId);
        if (!schedule.Any())
            return NotFound(new ApiResponse(404, "No schedule found for this doctor"));

        return Ok(schedule);
    }

    // GET api/DoctorSchedule/available-slots?date=2025-07-15&doctorId=5
    [HttpGet("available-slots")]
    public async Task<IActionResult> GetAvailableSlots([FromQuery] DateTime date, [FromQuery] int? doctorId = null)
    {
        int resolvedDoctorId;

        if (doctorId.HasValue && doctorId.Value > 0)
        {
            // Called by patient/booking — use the supplied ID directly
            resolvedDoctorId = doctorId.Value;
        }
        else
        {
            // Called by the logged-in doctor — resolve from JWT
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new ApiResponse(401, "User not authenticated"));

            var doctors = await _uow.Repository<Doctor>().FindAsync(d => d.UserId == userId);
            var doctor = doctors.FirstOrDefault();
            if (doctor == null)
                return NotFound(new ApiResponse(404, "Doctor not found"));

            resolvedDoctorId = doctor.Id;
        }

        var slots = await _scheduleService.GetAvailableSlotsAsync(resolvedDoctorId, date);
        if (!slots.Any())
            return NotFound(new ApiResponse(404, "No schedule found for this date"));

        return Ok(slots);
    }

    // POST api/DoctorSchedule
    [Authorize(Roles = "Admin, Doctor")]
    [HttpPost]
    public async Task<IActionResult> AddSchedule([FromBody] CreateDoctorScheduleDto dto)
    {
        if (User.IsInRole("Doctor"))
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Invalid token");

            var doctorId = await _scheduleService.GetDoctorIdByUserIdAsync(userId);
            if (doctorId == null)
                return NotFound(new ApiResponse(404, "Doctor profile not found"));

            dto.DoctorId = doctorId.Value;
        }

        if (User.IsInRole("Admin") && dto.DoctorId <= 0)
            return BadRequest(new ApiResponse(400, "DoctorId is required for Admin"));

        await _scheduleService.AddScheduleAsync(dto);
        return Ok(new { message = "Schedule added successfully" });
    }
}