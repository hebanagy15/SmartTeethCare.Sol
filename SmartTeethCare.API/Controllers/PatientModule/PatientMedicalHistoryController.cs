using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartTeethCare.Core.Interfaces.Services.PatientModule;
using System.Security.Claims;

namespace SmartTeethCare.API.Controllers.PatientModule
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Patient")]
    public class PatientMedicalHistoryController : ControllerBase
    {
        private readonly IPatientMedicalHistoryService _medicalHistoryService;

        public PatientMedicalHistoryController(IPatientMedicalHistoryService medicalHistoryService)
        {
            _medicalHistoryService = medicalHistoryService;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetMyMedicalHistory()
        {
            var patientId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var histories = await _medicalHistoryService.GetMyMedicalHistoryAsync(User);

            return Ok(histories);
        }
    }
}
