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

            // 1️⃣ تحويل الـ Dictionary إلى List منظمة
            var predictions = result.Predictions
                .Select(p => new
                {
                    Disease = p.Key,
                    Confidence = float.Parse(p.Value.Replace("%", ""))
                })
                .ToList();

            // 2️⃣ أعلى مرض
            var top = predictions
                .OrderByDescending(p => p.Confidence)
                .First();

            // 3️⃣ تحميل كل الـ specialities مرة واحدة (أفضل أداء)
            var allSpecialities = await _unitOfWork
                .Repository<Speciality>()
                .GetAllAsync();

            // 4️⃣ تخصص المرض الأعلى
            var speciality = allSpecialities
                .FirstOrDefault(s => s.Disease == top.Disease);

            // 5️⃣ اختيار دكتور عشوائي للتخصص
            var doctors = await _unitOfWork.Repository<Doctor>()
                .FindAsync(d => d.SpecialtyID == speciality.Id,
                    include: q => q.Include(d => d.User));

            var doctor = doctors?
                .OrderBy(d => Guid.NewGuid())
                .FirstOrDefault();

            // 6️⃣ باقي الأمراض (غير top)
            var others = predictions
                .Where(p => p.Disease != top.Disease)
                .Select(p =>
                {
                    var spec = allSpecialities
                        .FirstOrDefault(s => s.Disease == p.Disease);

                    return new
                    {
                        Disease = p.Disease,
                        Confidence = p.Confidence,
                        Speciality = spec?.Name
                    };
                })
                .ToList();

            // 7️⃣ Response النهائي
            return Ok(new
            {
                TopDisease = new
                {
                    Disease = top.Disease,
                    Confidence = top.Confidence,
                    Speciality = speciality?.Name ?? "No speciality available",
                    DoctorName = doctor?.User?.DisplayName
                            ?? doctor?.User?.UserName
                            ?? "No doctor available"
                },

                OtherPredictions = others,

                Disclaimer = result.Disclaimer
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
