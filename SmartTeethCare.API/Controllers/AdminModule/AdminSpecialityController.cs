using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartTeethCare.Core.DTOs.AdminModule;
using SmartTeethCare.Core.Interfaces.Services.AdminModule;
using System.Security.Claims;

namespace SmartTeethCare.API.Controllers.AdminModule
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminSpecialityController : ControllerBase
    {
        private readonly IAdminSpecialityService _service;

        public AdminSpecialityController(IAdminSpecialityService service)
        {
            _service = service;
        }

        private ClaimsPrincipal CurrentUser => User;

        // Secure GetAll
        [HttpGet(Name = "GetAllSpecialities")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }
        // Secure GetById
        [HttpGet("{id}", Name = "GetSpecialityById")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _service.GetByIdAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
        [HttpPost(Name = "CreateSpeciality")]
        public async Task<IActionResult> Create([FromBody] SpecialityDTO dto)
        {
            try
            {
                var result = await _service.CreateAsync(dto, CurrentUser);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpPut("{id}", Name = "UpdateSpeciality")]
        public async Task<IActionResult> Update(int id, [FromBody] SpecialityDTO dto)
        {
            if (!dto.Id.HasValue || id != dto.Id.Value)
                return BadRequest(new { Message = "Id is required and must match route" });

            try
            {
                await _service.UpdateAsync(dto, CurrentUser);
                return NoContent();
            }
            catch (Exception ex) when (ex is ArgumentException || ex is KeyNotFoundException)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpDelete("{id}", Name = "DeleteSpeciality")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteAsync(id, CurrentUser);
                return NoContent();
            }
            catch (Exception ex) when (ex is ArgumentException || ex is KeyNotFoundException)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }


    }
}
