using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.DTOs.AI_Services
{
    public class PredictionDto
    {
        public string Disease { get; set; }                   // caries
        public float Confidence { get; set; }                 // 0.85
    }
}
