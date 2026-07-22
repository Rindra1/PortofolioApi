using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using PortofolioApi.Domain.DTOs;
using PortofolioApi.Services;
using static System.Net.WebRequestMethods;

namespace PortofolioApi.Components.Shared;
public partial class ChatBot
{
    [Parameter]
    public LocalizationService Localizer { get; set; }
    [Inject]
    protected HttpClient Http { get; set; }
    [Inject]
    protected IJSRuntime JS { get; set; }

    private string UserMessage { get; set; } = "";
    private List<ChatEntry> ChatResponses { get; set; } = new();
    // Chatbot API endpoint
    private string ChatApiEndpoint => "api/chat";
    protected override void OnInitialized()
    {
        Localizer.OnChange += OnLangChanged;
        /*if (ChatResponses.Count == 0)
        {
            ChatResponses.Add(new ChatEntry
            {
                UserMessage = "",
                BotResponse = Localizer.T("MessageChat")
            });
        }*/
    }

    private void OnLangChanged()
    {
        InvokeAsync(async () =>
        {
            try
            {
                if (ChatResponses.Count > 0 && string.IsNullOrEmpty(ChatResponses[0].UserMessage))
                {
                    ChatResponses[0].BotResponse = Localizer.T("MessageChat");
                }
            }
            catch { }
            StateHasChanged();
        });
    }

    private async Task SendMessage()
    {
        if (string.IsNullOrWhiteSpace(UserMessage)) return;
        var userMsg = UserMessage;
        UserMessage = ""; 
        ChatResponses.Add(new ChatEntry
        {
            UserMessage = userMsg,
            BotResponse = ""
        });
        StateHasChanged();
        try
        {
            var response = await Http.PostAsJsonAsync(ChatApiEndpoint, new { message = userMsg });
            if (response.IsSuccessStatusCode)
            {
                var chatResponse = await response.Content.ReadFromJsonAsync<ChatResponseDTO>();
                var botMessage = chatResponse?.Message ?? "Aucune réponse";
                await TypeBotResponse(botMessage);
            }
            else
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                ChatResponses.Last().BotResponse = $"Erreur : {errorMsg}";
                StateHasChanged();
            }
        }
        catch (Exception ex)
        {
            ChatResponses.Last().BotResponse = $"Exception : {ex.Message}";
            StateHasChanged();
        }
        await ScrollChatToBottom();
    }

    private async Task SendSuggestion(string suggestion)
    {
        UserMessage = suggestion;
        await SendMessage();
    }

    private void OnChatKeyPress(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            SendMessage();
        }
    }

    private async Task TypeBotResponse(string botMessage)
    {
        var typedResponse = "";
        var lastEntry = ChatResponses.Last();
        foreach (var c in botMessage)
        {
            typedResponse += c;
            lastEntry.BotResponse = typedResponse;
            StateHasChanged();
            await Task.Delay(20);
        }
        await ScrollChatToBottom();
    }

    private async Task ScrollChatToBottom()
    {
        try
        {
            await JS.InvokeVoidAsync("eval", @"
                var chatContainer = document.querySelector('#chatMessagesFloat');
                if(chatContainer) chatContainer.scrollTop = chatContainer.scrollHeight;
            ");
        }
        catch { }
    }

    private class ChatEntry
    {
        public string UserMessage { get; set; } = "";
        public string BotResponse { get; set; } = "";
    }

    private class ChatResponseDTO
    {
        public string Message { get; set; } = "";
    }
}
