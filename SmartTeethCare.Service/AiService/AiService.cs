using Microsoft.AspNetCore.Http;
using SmartTeethCare.Core.DTOs.AI_Services;
using SmartTeethCare.Core.Interfaces.Services.AiService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartTeethCare.Service.AiService
{
    public class AiService : IAiService
    {
        private readonly HttpClient _httpClient;

        public AiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://mennaelaraqi-teeth-api.hf.space");
        }

        public async Task<PredictResponseDto> PredictAsync(IFormFile image)
        {
            using var content = new MultipartFormDataContent();

            var stream = image.OpenReadStream();
            var fileContent = new StreamContent(stream);
            content.Add(fileContent, "file", image.FileName);

            var response = await _httpClient.PostAsync("/predict", content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<PredictResponseDto>(json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }

        public async Task<ChatResponseDto> ChatAsync(ChatRequestDto dto)
        {

            var response = await _httpClient.PostAsJsonAsync("/chat", dto);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(body);
            }
            return await response.Content.ReadFromJsonAsync<ChatResponseDto>();
        }
    }
}
