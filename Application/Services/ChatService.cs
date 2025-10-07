using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using PortofolioApi.Domain.DTOs;

namespace PortofolioApi.Application.Services;

    public class ChatService
    {
        private readonly HttpClient _httpClient;
        
        public ChatService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("HUGGINGFACE_API_KEY"));
        }

        public async Task<string> SendMessageAsync(string prompt)
        {
            var data = new
            {
                model = "openai/gpt-oss-120b:fireworks-ai",
                messages = new[]
                {
                    new { role = "user", content = prompt }
                }
            };

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(
                "https://router.huggingface.co/v1/chat/completions",
                content
            );

            var responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Erreur Hugging Face: {response.StatusCode}, {responseText}");

            var hfResponse = JsonSerializer.Deserialize<HuggingFaceResponse>(responseText);

            return hfResponse?.choices?[0]?.message?.content ?? "";
            // Récupère uniquement le texte généré
            /*using var doc = JsonDocument.Parse(responseText);
            var msg = doc.RootElement
                         .GetProperty("choices")[0]
                         .GetProperty("message")
                         .GetProperty("content")[0]
                         .GetString();

            return msg ?? "Aucune réponse";*/
        }
    }


