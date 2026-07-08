using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.DTOs.HomeService
{
    public class TopReviewDto
    {
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
    }
}
