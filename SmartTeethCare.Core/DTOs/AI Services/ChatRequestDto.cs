using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.DTOs.AI_Services
{
    public class ChatRequestDto
    {
        public string Disease { get; set; }                   // caries
        public string User_Message { get; set; }              // "What are the symptoms of caries and how can I prevent it?"
    }
}
