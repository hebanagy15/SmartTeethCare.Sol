using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartTeethCare.Core.DTOs.AI_Services;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Interfaces.Services.AiService;
using SmartTeethCare.Core.Interfaces.UnitOfWork;

namespace SmartTeethCare.API.Controllers.AiService
{
    [Route("api/[controller]")]
    [ApiController]
    public class AiServiceController : ControllerBase
    {
        private readonly IAiService _aiService;
        private readonly IUnitOfWork _unitOfWork;

        public AiServiceController(IAiService aiService , IUnitOfWork unitOfWork )
        {
            _aiService = aiService;
            _unitOfWork = unitOfWork;
        }
        [HttpPost("analyze")]
        public async Task<IActionResult> Analyze(IFormFile image)
        {
            var result = await _aiService.PredictAsync(image);

            if (!result.Is_Teeth)
                return BadRequest("The image is unclear or does not seem to be a valid teeth image. Please upload a clearer one.");

            // top disease = top Confidence
            var topDisease = result.Predictions
                .OrderByDescending(p => p.Confidence)
                .FirstOrDefault()?.Disease;

            var speciality = (await _unitOfWork.Repository<Speciality>().FindAsync(s => s.Disease == topDisease)).FirstOrDefault();
            if (speciality == null)
            {
                return NotFound("No speciality found for this disease");
            }
            var doctors = await _unitOfWork.Repository<Doctor>().FindAsync(d => d.SpecialtyID == speciality.Id,include: q => q.Include(d => d.User));
            if (doctors == null || !doctors.Any())
            {
                return Ok(new
                {
                    Disease = topDisease,
                    Message = "No doctors available for this speciality"
                });
            }
            var doctor = doctors.OrderBy(d => Guid.NewGuid()).FirstOrDefault();
            return Ok(new
            {
                Disease = topDisease,
                Predictions = result.Predictions,
                Disclaimer = result.Disclaimer,
                Speciality = speciality.Name,
                DoctorName = doctor?.User?.UserName
            });
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat(ChatRequestDto dto)
        {
            var response = await _aiService.ChatAsync(dto.Disease, dto.User_Message);

            return Ok(new
            {
                Message = response
            });
        }
    }
}
