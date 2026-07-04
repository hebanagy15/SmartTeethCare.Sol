using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.DTOs.AI_Services
{
    public class ChatRequestDto
    {
        [JsonPropertyName("disease")]
        public string Disease { get; set; }                   // caries

        [JsonPropertyName("user_message")]
        public string User_Message { get; set; }              // "What are the symptoms of caries and how can I prevent it?"

        [JsonPropertyName("session_id")]
        public string SessionId { get; set; } = string.Empty;
    }
}
