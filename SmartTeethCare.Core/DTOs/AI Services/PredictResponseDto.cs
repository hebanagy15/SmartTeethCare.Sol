using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.DTOs.AI_Services
{
    public class PredictResponseDto
    {
        [JsonPropertyName("is_teeth")]
        public bool Is_Teeth { get; set; }                 // true if the image contains teeth, false otherwise

        public Dictionary<string, string> Predictions { get; set; }   // Dictionary of predicted diseases and their confidence scores
        public string Disclaimer { get; set; }                   // A disclaimer message about the predictions
    }
}
