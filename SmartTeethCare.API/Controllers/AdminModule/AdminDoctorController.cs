using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTeethCare.Core.DTOs.AdminModule;
using SmartTeethCare.Core.Interfaces.Services.AdminModule;

namespace SmartTeethCare.API.Controllers.AdminModule
{
    [Authorize(Roles = "Admin")]
    [Route("api/admin/doctors")]
    [ApiController]
    public class AdminDoctorController : ControllerBase
    {
        private readonly IAdminDoctorService _service;

        public AdminDoctorController(IAdminDoctorService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllDoctorsAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
            => Ok(await _service.GetDoctorByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create(CreateDoctorDto dto)
        {
            await _service.AddDoctorAsync(dto);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateDoctorDto dto)
        {
            await _service.UpdateDoctorAsync(id, dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteDoctorAsync(id);
            return Ok();
        }

        [HttpPut("toggle-status/{id}")]
        public async Task<IActionResult> ToggleDoctorStatus(int id, bool cancelAppointments = false)
        {
            await _service.ToggleDoctorStatusAsync(id, cancelAppointments);

            return Ok(new
            {
                Message = "Doctor status updated successfully."
            });
        }
    }

}
