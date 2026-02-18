using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartTeethCare.Core.DTOs.Lookup;
using SmartTeethCare.Core.Interfaces.Services.Lookup;
using SmartTeethCare.Service.Lookup;

namespace SmartTeethCare.API.Controllers.Lookup
{
    [Route("api/[controller]")]
    [ApiController]
    public class LookupController : ControllerBase
    {
        private readonly ILookupService _lookupService;

        public LookupController(ILookupService lookupService)
        {
            _lookupService = lookupService;
        }

        // GET: api/Lookup/Doctors
        [HttpGet("Doctors")]
        public async Task<ActionResult<IEnumerable<DoctorDTO>>> GetDoctors()
        {
            var doctors = await _lookupService.GetDoctorsAsync();
            return Ok(doctors);
        }

        // GET http://localhost:5039/api/Lookup/DoctorsBySpeciality/3

        [HttpGet("DoctorsBySpeciality/{specialityId}")]
        public async Task<IActionResult> GetDoctorsBySpeciality(int specialityId)
        {
            var doctors = await _lookupService.GetDoctorsBySpecialityAsync(specialityId);
            return Ok(doctors);
        }


        // GET: api/Lookup/Specializations
        [HttpGet("Specializations")]
        public async Task<ActionResult<IEnumerable<SpecializationDTO>>> GetSpecializations()
        {
            var specializations = await _lookupService.GetSpecializationsAsync();
            return Ok(specializations);
        }
    }
}
