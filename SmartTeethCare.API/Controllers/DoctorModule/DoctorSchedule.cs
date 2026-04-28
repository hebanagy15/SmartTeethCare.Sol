using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTeethCare.API.Errors;
using SmartTeethCare.Core.DTOs.DoctorModule;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Interfaces.Services.DoctorModule;
using System.Security.Claims;

namespace SmartTeethCare.API.Controllers.DoctorModule
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorScheduleController : ControllerBase
    {
        private readonly IDoctorScheduleService _scheduleService;

        public DoctorScheduleController(IDoctorScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        // GET api/doctorschedule/5
        [HttpGet("{doctorId}")]
        public async Task<IActionResult> GetDoctorSchedule(int doctorId)
        {
            var schedule = await _scheduleService.GetDoctorScheduleAsync(doctorId);

            if (!schedule.Any())
                return NotFound(new ApiResponse(404, "No schedule found for this doctor"));

            return Ok(schedule);
        }

        // GET api/doctorschedule/5/available-slots?date=2025-07-15
        [HttpGet("{doctorId}/available-slots")]
        public async Task<IActionResult> GetAvailableSlots(int doctorId, [FromQuery] DateTime date)
        {
            var slots = await _scheduleService.GetAvailableSlotsAsync(doctorId, date);

            if (!slots.Any())
                return NotFound(new ApiResponse(404, "No schedule found for this date"));

            return Ok(slots);
        }

        // POST api/doctorschedule
        [Authorize(Roles = "Admin, Doctor")]
        [HttpPost]
        public async Task<IActionResult> AddSchedule([FromBody] CreateDoctorScheduleDto dto)
        {
            // If Doctor — get DoctorId from JWT Token via Service
            if (User.IsInRole("Doctor"))
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("Invalid token");

                // ✅ Ask the Service (not UnitOfWork directly!)
                var doctorId = await _scheduleService.GetDoctorIdByUserIdAsync(userId);

                if (doctorId == null)
                    return NotFound(new ApiResponse(404, "Doctor profile not found"));

                // Override DoctorId — Doctor cannot set another doctor's schedule
                dto.DoctorId = doctorId.Value;
            }

            // If Admin — DoctorId must be provided in the request body
            if (User.IsInRole("Admin") && dto.DoctorId <= 0)
                return BadRequest(new ApiResponse(400, "DoctorId is required for Admin"));

            await _scheduleService.AddScheduleAsync(dto);
            return Ok(new { message = "Schedule added successfully" });
        }
    }
}
