using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartTeethCare.Core.DTOs.Pharmacy;
using SmartTeethCare.Core.Interfaces.Services.Pharmacy;

namespace SmartTeethCare.API.Controllers.PharmacyModule
{
    [Route("api/[controller]")]
    [ApiController]
    public class PharmacyController : ControllerBase
    {
        private readonly IPharmacyService _service; 
        public PharmacyController(IPharmacyService service) 
        { 
            _service = service; 
        }

        [HttpGet("Get-All-Pharmacies")]
        [Authorize]
        public async Task<IActionResult> GetAll() 
        { 
            var result = await _service.GetAllAsync(); 
            return Ok(result); 
        }

        [HttpGet("Get-Pharmacy-Details/{id}")]
        [Authorize] 
        public async Task<IActionResult> GetById(int id) 
        { 
            var result = await _service.GetByIdAsync(id); 
            return Ok(result);
        }

        [HttpPost("Create-Pharmacy")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> Create(CreatePharmacyDto dto) 
        { 
            await _service.AddAsync(dto); 
            return Ok(new { message = "Pharmacy created successfully." }); 
        }

        [HttpPut("Update-Pharmacy")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> Update(UpdatePharmacyDto dto) 
        { 
            await _service.UpdateAsync(dto); 
            return Ok(new { message = "Pharmacy updated successfully." }); 
        }

        [HttpDelete("Delete-Pharmacy/{id}")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> Delete(int id) 
        { 
            await _service.DeleteAsync(id); 
            return Ok(new { message = "Pharmacy deleted successfully." }); 
        }
    }
}
