using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTeethCare.Core.DTOs.MedicalRecordModule;
using SmartTeethCare.Core.Interfaces.Services.MedicalRecordModule;
using System.Security.Claims;

namespace SmartTeethCare.API.Controllers.MedicalRecord
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicalRecordsController : ControllerBase
    {
        private readonly IMedicalRecordService _service;

        public MedicalRecordsController(IMedicalRecordService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost("create")]
        public async Task<IActionResult> Create(CreateMedicalRecordDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                 ?? throw new Exception("User not authenticated");
            await _service.AddAsync(dto, userId);

            return Ok("Medical record created");
        }

        [Authorize(Roles = "Patient")]
        [HttpGet("my-records")]
        public async Task<IActionResult> GetMyMedicalRecords()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? throw new Exception("User not authenticated");

            var result = await _service.GetMyMedicalRecords(userId);

            return Ok(result);
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet("my-created-medical-records")]
        public async Task<IActionResult> GetMyCreatedMedicalRecords()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? throw new Exception("User not authenticated");

            var result = await _service.GetMyCreatedMedicalRecords(userId);

            return Ok(result);
        }


        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetMedicalRecordsDetails(int id)
        {
            var result = await _service.GetDetailsAsync(id);
            if (result == null)
                return NotFound("Medical record not found.");
            return Ok(result);
        }
    }
}
