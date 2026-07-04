using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartTeethCare.Core.Interfaces.Services.Pharmacy;

namespace SmartTeethCare.API.Controllers.PharmacyModule
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PharmacySearchController : ControllerBase
    {
        private readonly IPharmacySearchService _pharmacySearchService;

        public PharmacySearchController(IPharmacySearchService pharmacySearchService)
        {
            _pharmacySearchService = pharmacySearchService;
        }

        [HttpGet("available-pharmacies")]
        public async Task<IActionResult> GetAvailablePharmacies([FromQuery] int medicineId,[FromQuery] double latitude,[FromQuery] double longitude)
        {
            var result = await _pharmacySearchService.GetAvailablePharmaciesAsync(medicineId,latitude,longitude);

            return Ok(result);
        }
    }
}
