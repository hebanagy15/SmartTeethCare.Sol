using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartTeethCare.Core.Interfaces.Services.HomeService;

namespace SmartTeethCare.API.Controllers.Home
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IHomeService _homeService;

        public HomeController(IHomeService homeService)
        {
            _homeService = homeService;
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            var result = await _homeService.GetStatisticsAsync();
            return Ok(result);
        }

        [HttpGet("top-reviews")]
        public async Task<IActionResult> GetTopReviews()
        {
            var result = await _homeService.GetTopReviewsAsync();
            return Ok(result);
        }
    }
}
