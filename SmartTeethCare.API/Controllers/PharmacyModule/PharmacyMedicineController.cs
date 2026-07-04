using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTeethCare.Core.DTOs.Pharmacy;
using SmartTeethCare.Core.Interfaces.Services.Pharmacy;

namespace SmartTeethCare.API.Controllers.PharmacyModule
{
    [Route("api/[controller]")]
    [ApiController]
    public class PharmacyMedicineController : ControllerBase
    {
        private readonly IPharmacyMedicineService _service;

        public PharmacyMedicineController(IPharmacyMedicineService service)
        {
            _service = service;
        }

        [HttpGet("Get-All")]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("Get-By-Id/{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost("Create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreatePharmacyMedicineDto dto)
        {
            await _service.AddAsync(dto);
            return Ok(new
            {
                message = "Medicine added to pharmacy successfully."
            });
        }

        [HttpPut("Update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(UpdatePharmacyMedicineDto dto)
        {
            await _service.UpdateAsync(dto);
            return Ok(new
            {
                message = "Stock updated successfully."
            });
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int pharmacyId, int medicineId)
        {
            await _service.DeleteAsync(pharmacyId, medicineId);
            return Ok(new { message = "Deleted successfully." });
        }
    }
}