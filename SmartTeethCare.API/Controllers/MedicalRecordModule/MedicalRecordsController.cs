using Microsoft.AspNetCore.Mvc;
using SmartTeethCare.Core.DTOs.MedicalRecordModule;
using SmartTeethCare.Core.Interfaces.Services.MedicalRecordModule;

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

        [HttpPost]
        public async Task<IActionResult> Create(CreateMedicalRecordDto dto)
        {
            await _service.AddAsync(dto);
            return Ok("Medical record created");
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetByPatient(int patientId)
        {
            var result = await _service.GetByPatientId(patientId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetById(id);
            return Ok(result);
        }
    }
}
