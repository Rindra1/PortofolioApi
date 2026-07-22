using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace PortofolioApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TranslateController : ControllerBase
{
    private readonly IHttpClientFactory _httpFactory;

    public TranslateController(IHttpClientFactory httpFactory)
    {
        _httpFactory = httpFactory;
    }

    public class TranslateRequest
    {
        public List<string>? Texts { get; set; }
        public string? Target { get; set; }
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] TranslateRequest req)
    {
        if (req?.Texts == null || req.Texts.Count == 0 || string.IsNullOrEmpty(req.Target))
            return BadRequest("Missing texts or target");

        var client = _httpFactory.CreateClient();

        var translations = new List<string>();

        // Use LibreTranslate public instance as fallback
        var apiUrl = "https://libretranslate.com/translate";

        foreach (var t in req.Texts)
        {
            try
            {
                var body = new Dictionary<string, string>()
                {
                    ["q"] = t ?? string.Empty,
                    ["source"] = "auto",
                    ["target"] = req.Target,
                    ["format"] = "text"
                };

                var response = await client.PostAsync(apiUrl, new FormUrlEncodedContent(body));
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
                    if (json.TryGetProperty("translatedText", out var translated))
                    {
                        translations.Add(translated.GetString() ?? string.Empty);
                        continue;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error translating '{t}': {ex.Message}");
                // ignore and fallback
            }
            // fallback: return original
            translations.Add(t ?? string.Empty);
        }

        return Ok(translations);
    }
}
