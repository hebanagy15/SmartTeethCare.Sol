using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartTeethCare.Core.DTOs.PatientModule;
using SmartTeethCare.Core.Interfaces.Services.PatientModule;

namespace SmartTeethCare.API.Controllers.PatientModule
{
    [Authorize(Roles = "Patient")]
    [Route("api/[controller]")]
    [ApiController]
    public class PatientReviewsController : ControllerBase
    {
        private readonly IPatientReviewService _service;

        public PatientReviewsController(IPatientReviewService service)
        {
            _service = service;
        }

        // POST: api/PatientReviews/AddReview

        [HttpPost("AddReview")]
        public async Task<IActionResult> AddReview(AddReviewDTO dto)
        {
            await _service.AddReviewAsync(dto, User);
            return Ok("Review added successfully");
        }

        // PUT: api/PatientReviews/UpdateReview/{reviewId}

        [HttpPut("UpdateReview/{reviewId}")]
        public async Task<IActionResult> UpdateReview(int reviewId, UpdateReviewDTO dto)
        {
            await _service.UpdateReviewAsync(reviewId, dto, User);
            return Ok("Review updated successfully");
        }

        // DELETE: api/PatientReviews/DeleteReview/{reviewId}

        [HttpDelete("DeleteReview/{reviewId}")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            await _service.DeleteReviewAsync(reviewId, User);
            return Ok("Review deleted successfully");
        }

        // GET: api/PatientReviews/GetMyReviews

        [HttpGet("GetMyReviews")]
        public async Task<IActionResult> GetMyReviews()
        {
            var result = await _service.GetMyReviewsAsync(User);
            return Ok(result);
        }

        // GET: api/PatientReviews/GetMyReviewsForDoctor/{doctorId}

        [HttpGet("GetMyReviewsForDoctor/{doctorId}")]
        public async Task<IActionResult> GetMyReviewsForDoctor(int doctorId)
        {
            var result = await _service.GetMyReviewsForDoctorAsync(doctorId, User);
            return Ok(result);
        }

        

    }
}




    
