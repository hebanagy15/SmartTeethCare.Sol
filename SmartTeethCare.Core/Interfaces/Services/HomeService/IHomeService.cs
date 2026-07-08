using SmartTeethCare.Core.DTOs.HomeService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.Interfaces.Services.HomeService
{
    public interface IHomeService
    {
        Task<HomeStatisticsDto> GetStatisticsAsync();
        Task<List<TopReviewDto>> GetTopReviewsAsync();
    }
}
