using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.DTOs.AI_Services
{
    public class PredictResponseDto
    {
        public bool Is_Teeth { get; set; }                 // true if the image contains teeth, false otherwise
        public List<PredictionDto> Predictions { get; set; }     // List of predicted diseases and their confidence scores
        public string Disclaimer { get; set; }                   // A disclaimer message about the predictions
    }
}
