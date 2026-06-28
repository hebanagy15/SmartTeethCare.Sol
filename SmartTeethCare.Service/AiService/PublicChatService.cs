using SmartTeethCare.Core.DTOs.AI_Services;
using SmartTeethCare.Core.Interfaces.Services.AiService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Service.AiService
{
    public class PublicChatService : IPublicChatService
    {
        private readonly HttpClient _httpClient;

        public PublicChatService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress =
                new Uri("https://mennaelaraqi-chatbot-endpoint.hf.space");
        }

        public async Task<PublicChatResponseDto> ChatAsync(PublicChatRequestDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("/chat", dto);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<PublicChatResponseDto>();
        }
    }
}
