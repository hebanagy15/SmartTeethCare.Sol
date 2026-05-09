using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTeethCare.Core.DTOs.PatientModule;
using SmartTeethCare.Core.Interfaces.Services.PatientModule;
using System.Security.Claims;

namespace SmartTeethCare.API.Controllers.PatientModule
{
    [Authorize(Roles = "Patient")]
    [ApiController]
    [Route("api/patient/profile")]
    public class PatientProfileController : ControllerBase
    {
        private readonly IPatientProfileService _patientProfileService;

        public PatientProfileController(
            IPatientProfileService patientProfileService)
        {
            _patientProfileService = patientProfileService;
        }
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result =
                await _patientProfileService.GetProfileAsync(userId);

            return Ok(result);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdatePatientProfileDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            await _patientProfileService.UpdateProfileAsync(
                userId,
                dto);

            return Ok(new
            {
                Message = "Profile updated successfully"
            });
        }
    }
}