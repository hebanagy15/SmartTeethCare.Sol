using Microsoft.AspNetCore.Http;
using SmartTeethCare.Core.DTOs.AI_Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.Interfaces.Services.AiService
{
    public interface IAiService
    {
        Task<PredictResponseDto> PredictAsync(IFormFile image);
        Task<string> ChatAsync(string disease, string message);
    }
}
