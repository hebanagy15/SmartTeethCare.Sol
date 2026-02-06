using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartTeethCare.API.Errors;

namespace SmartTeethCare.API.Controllers.Errors
{
    [Route("/errors/{code}")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)] // to hide this controller from swagger documentation
    public class ErrorsController : ControllerBase
    {
        public ActionResult Error(int code)
        {
            return NotFound(new ApiResponse(code));
        }
    }
}
