using SmartTeethCare.Core.DTOs.AI_Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.Interfaces.Services.AiService
{
    public interface IPublicChatService
    {
        Task<PublicChatResponseDto> ChatAsync(PublicChatRequestDto dto);
    }
}
