using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartTeethCare.Core.DTOs.AdminModule;
using SmartTeethCare.Core.Interfaces.Services.AdminModule;

namespace SmartTeethCare.API.Controllers.AdminModule
{
    [Route("api/adminPatient")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class AdminPatientController : ControllerBase
    {
        private readonly IAdminPatientService _adminPatientService;

        public AdminPatientController(IAdminPatientService adminPatientService)
        {
            _adminPatientService = adminPatientService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePatient([FromBody] CreatePatientByAdminDTO dto)
        {
            var tempPassword = await _adminPatientService.CreatePatientAsync(dto);

            return Ok(new
            {
                Message = "Patient created successfully",
                TemporaryPassword = tempPassword
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var patients = await _adminPatientService.GetAllAsync();
            return Ok(patients);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var patient = await _adminPatientService.GetByIdAsync(id);
            return Ok(patient);
        }
    }
}
