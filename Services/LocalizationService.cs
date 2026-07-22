using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Globalization;
using PortofolioApi.Resources;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace PortofolioApi.Services
{
    public class LocalizationService
    {
        private readonly IJSRuntime _js;
        private readonly System.Net.Http.IHttpClientFactory _httpFactory;
        public string CurrentLanguage { get; private set; } = "fr";

        public event Action? OnChange;
        private bool _initialized = false;

        private readonly Dictionary<string, Dictionary<string, string>> _dict = new()
        {
            ["fr"] = new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Home"] = "Accueil",
                ["About"] = "À propos de moi",
                ["Projects"] = "Mes projets",
                ["Skills"] = "Compétences",
                ["Experience"] = "Expériences",
                ["Contact"] = "Contact",
                ["SeeProjects"] = "Voir mes projets",
                ["ViewDetails"] = "Voir détails",
                ["ChatGreeting"] = "Bonjour! Comment puis-je vous aider aujourd'hui?",
                ["ChatPlaceholder"] = "Écrire un message...",
                ["Send"] = "Envoyer",
                ["Present"] = "Présent"
            },
            ["en"] = new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Home"] = "Home",
                ["About"] = "About me",
                ["Projects"] = "Projects",
                ["Skills"] = "Skills",
                ["Experience"] = "Experience",
                ["Contact"] = "Contact",
                ["SeeProjects"] = "See my projects",
                ["ViewDetails"] = "See details",
                ["ChatGreeting"] = "Hello! How can I help you today?",
                ["ChatPlaceholder"] = "Write a message...",
                ["Send"] = "Send",
                ["Present"] = "Present"
            }
        };

        public LocalizationService(IJSRuntime js, System.Net.Http.IHttpClientFactory httpFactory)
        {
            _js = js;
            _httpFactory = httpFactory;
        }

        public string T(string key)
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;
            try
            {
                // Try resource manager first (Resources/Resource.resx + localized variants)
                var culture = new CultureInfo(CurrentLanguage ?? "fr");
                var res = Resource.ResourceManager.GetString(key, culture);
                if (!string.IsNullOrEmpty(res)) return res;
                // fallback to invariant/base resources (no culture)
                res = Resource.ResourceManager.GetString(key, CultureInfo.InvariantCulture);
                if (!string.IsNullOrEmpty(res)) return res;
            }
            catch
            {
                // ignore resource lookup errors and fall back to dictionary
            }

            // Fallback to in-memory dictionary (legacy)
            if (!string.IsNullOrEmpty(CurrentLanguage) && _dict.TryGetValue(CurrentLanguage, out var map) && map.TryGetValue(key, out var val))
                return val;
            // fallback to french dictionary
            //if (_dict.TryGetValue("fr", out var fallback) && fallback.TryGetValue(key, out var fv)) return fv;
            return key;
        }

        public async Task InitializeAsync()
        {
            if(_initialized) return;
            try
            {
                string detectedLang = "fr";
                var saved = await _js.InvokeAsync<string>("localStorage.getItem", "lang");
                if (!string.IsNullOrEmpty(saved))
                {
                    detectedLang = saved;
                }
                else
                {
                    // detect browser language
                    var nav = await _js.InvokeAsync<string>("eval", "navigator.language || navigator.userLanguage");
                    if (!string.IsNullOrEmpty(nav) && nav.Length >= 2)
                    {
                        var code = nav.Substring(0,2).ToLower();
                        if (_dict.ContainsKey(code)) detectedLang = code;
                    }
                }

                if(detectedLang != CurrentLanguage)
                {
                    CurrentLanguage = detectedLang;
                }
                
                try { Resource.Culture = new CultureInfo(CurrentLanguage); } catch { } 
                _initialized = true;
                await _js.InvokeVoidAsync("localStorage.setItem", "lang", CurrentLanguage);
                
            }
            catch
            {
                
                _initialized = true;
                // ignore JS errors
            }
        }

        public async Task SetLanguageAsync(string lang)
        {
            if (string.IsNullOrEmpty(lang)) return;
            lang = lang.ToLower();
            // normalize locales like en-GB, en-US -> en
            if (lang.Contains('-')) lang = lang.Split('-')[0];
            if (lang.StartsWith("en")) lang = "en";
            if (lang.StartsWith("fr")) lang = "fr";
            if (!_dict.ContainsKey(lang)) return;
            CurrentLanguage = lang;
            try { await _js.InvokeVoidAsync("localStorage.setItem", "lang", lang); } catch { }
            try { Resource.Culture = new CultureInfo(CurrentLanguage); } catch { }
            OnChange?.Invoke();
        }

        // Translate an array of texts using public translation endpoints with fallbacks
        public async Task<List<string>> TranslateTextsAsync(List<string> texts, string target)
        {
            var result = new List<string>();
            if (texts == null || texts.Count == 0) return result;
            try
            {
                var client = _httpFactory.CreateClient();

                var endpoints = new[]
                {
                    //"https://translate.argosopentech.com/translate",
                    "https://libretranslate.de/translate",
                    "https://libretranslate.com/translate"
                };

                foreach (var t in texts)
                {
                    var translatedThis = false;

                    // local helpers to preserve separators during translation
                    string ApplyPlaceholders(string src, out Dictionary<string, string> map)
                    {
                        map = new Dictionary<string, string>();
                        if (string.IsNullOrEmpty(src)) return string.Empty;
                        // patterns to preserve (longer patterns first to avoid partial matches)
                        var patterns = new[] { "\r\n", "\n", "<br/>", "<br>", "|", "/" };
                        Array.Sort(patterns, (a, b) => b.Length.CompareTo(a.Length));

                        var sb = new System.Text.StringBuilder();
                        int i = 0;
                        while (i < src.Length)
                        {
                            bool matched = false;
                            foreach (var p in patterns)
                            {
                                if (i + p.Length <= src.Length && string.Compare(src, i, p, 0, p.Length, StringComparison.Ordinal) == 0)
                                {
                                    var token = "__PH_" + Guid.NewGuid().ToString("N") + "__";
                                    sb.Append(token);
                                    map[token] = p;
                                    i += p.Length;
                                    matched = true;
                                    break;
                                }
                            }
                            if (!matched)
                            {
                                sb.Append(src[i]);
                                i++;
                            }
                        }

                        return sb.ToString();
                    }

                    string RestorePlaceholders(string translated, Dictionary<string, string> map)
                    {
                        if (string.IsNullOrEmpty(translated) || map == null || map.Count == 0) return translated ?? string.Empty;
                        foreach (var kv in map)
                        {
                            translated = translated.Replace(kv.Key, kv.Value);
                        }
                        return translated;
                    }


                    // First try MyMemory (less likely to be blocked)
                    /*try
                    {
                        var src = "fr"; // assume original texts are French
                        var mmUrl = $"https://api.mymemory.translated.net/get?q={Uri.EscapeDataString(t ?? string.Empty)}&langpair={src}|{target}";
                        var mmResp = await client.GetAsync(mmUrl);
                        if (mmResp.IsSuccessStatusCode)
                        {
                            var mmText = await mmResp.Content.ReadAsStringAsync();
                            if (!string.IsNullOrWhiteSpace(mmText) && !mmText.TrimStart().StartsWith("<"))
                            {
                                try
                                {
                                    using var doc = System.Text.Json.JsonDocument.Parse(mmText);
                                    var root = doc.RootElement;
                                    if (root.TryGetProperty("responseData", out var rd) && rd.ValueKind == System.Text.Json.JsonValueKind.Object)
                                    {
                                        if (rd.TryGetProperty("translatedText", out var trans))
                                        {
                                            var s = trans.GetString() ?? string.Empty;
                                            result.Add(s);
                                            translatedThis = true;
                                        }
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                    catch { }

                    if (translatedThis)
                    {
                        Console.WriteLine("Used MyMemory for translation");
                        continue;
                    }*/

                    // Otherwise try Libre/argos endpoints
                    /*foreach (var apiUrl in endpoints)
                    {
                        try
                        {
                            var body = new Dictionary<string, string>()
                            {
                                ["q"] = t ?? string.Empty,
                                ["source"] = "auto",
                                ["target"] = target,
                                ["format"] = "text"
                            };

                            // prefer JSON payloads (some hosts block form posts)
                            var payload = new { q = t ?? string.Empty, source = "auto", target = target, format = "text" };
                            HttpResponseMessage response;
                            try
                            {
                                response = await client.PostAsJsonAsync(apiUrl, payload);
                            }
                            catch
                            {
                                // fallback to form encoded
                                using var reqContent = new System.Net.Http.FormUrlEncodedContent(body);
                                var req = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Post, apiUrl)
                                {
                                    Content = reqContent
                                };
                                req.Headers.Accept.Clear();
                                req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                req.Headers.UserAgent.ParseAdd("Mozilla/5.0 (compatible; PortfolioApp/1.0)");
                                response = await client.SendAsync(req);
                            }

                            if (!response.IsSuccessStatusCode) continue;

                            var textResp = await response.Content.ReadAsStringAsync();
                            // detect HTML responses (blocked page) quickly
                            if (!string.IsNullOrWhiteSpace(textResp) && textResp.TrimStart().StartsWith("<"))
                            {
                                Console.WriteLine($"Endpoint {apiUrl} returned HTML, skipping. Snippet: {textResp.Trim().Substring(0, Math.Min(200, textResp.Length))}");
                                continue;
                            }
                            if (string.IsNullOrWhiteSpace(textResp)) continue;

                            try
                            {
                                using var doc = System.Text.Json.JsonDocument.Parse(textResp);
                                var root = doc.RootElement;
                                if (root.ValueKind == System.Text.Json.JsonValueKind.Object)
                                {
                                    if (root.TryGetProperty("translatedText", out var tt))
                                    {
                                        result.Add(tt.GetString() ?? string.Empty);
                                        translatedThis = true;
                                        Console.WriteLine($"Used endpoint {apiUrl} for translation");
                                        break;
                                    }
                                    if (root.TryGetProperty("result", out var tr))
                                    {
                                        result.Add(tr.GetString() ?? string.Empty);
                                        translatedThis = true;
                                        Console.WriteLine($"Used endpoint {apiUrl} for translation (result)");
                                        break;
                                    }
                                }
                            }
                            catch
                            {
                                // parse error -> try next endpoint
                            }
                        }
                        catch
                        {
                            // try next endpoint
                        }
                    }*/

                    if (!translatedThis)
                    {
                        // Fallback: try Google unofficial translate endpoint
                        try
                        {
                            var tToSend = ApplyPlaceholders(t, out var phMap);
                            var gUrl = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl=auto&tl={target}&dt=t&q={Uri.EscapeDataString(tToSend ?? string.Empty)}";
                            var gResp = await client.GetAsync(gUrl);
                            if (gResp.IsSuccessStatusCode)
                            {
                                var gText = await gResp.Content.ReadAsStringAsync();
                                if (!string.IsNullOrWhiteSpace(gText) && !gText.TrimStart().StartsWith("<"))
                                {
                                    try
                                    {
                                        using var doc = System.Text.Json.JsonDocument.Parse(gText);
                                        // Expected structure: [[ ["translated", ...], ... ], ...]
                                        if (doc.RootElement.ValueKind == System.Text.Json.JsonValueKind.Array && doc.RootElement.GetArrayLength() > 0)
                                        {
                                            var first = doc.RootElement[0];
                                            if (first.ValueKind == System.Text.Json.JsonValueKind.Array && first.GetArrayLength() > 0)
                                            {
                                                var seg = first[0];
                                                if (seg.ValueKind == System.Text.Json.JsonValueKind.Array && seg.GetArrayLength() > 0)
                                                {
                                                    var translated = seg[0].GetString();
                                                    if (!string.IsNullOrEmpty(translated))
                                                    {
                                                        var restored = RestorePlaceholders(translated, phMap);
                                                        result.Add(restored);
                                                        translatedThis = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    catch { }
                                }
                            }
                        }
                        catch { }

                        if (!translatedThis)
                        {
                            result.Add(t ?? string.Empty);
                        }
                    }
                }
            }
            catch
            {
                // ignore outer errors
            }
            return result;
        }
    }
}
