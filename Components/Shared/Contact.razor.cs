using Microsoft.AspNetCore.Components;
using PortofolioApi.Domain.DTOs;
using PortofolioApi.Services;
using Microsoft.JSInterop;
using static System.Net.WebRequestMethods;

namespace PortofolioApi.Components.Shared
{
    public partial class Contact
    {
        [Parameter]
        public UtilisateurDTO portfolio { get; set; }
        [Parameter]
        public LocalizationService Localizer { get; set; }
        [Inject]
        public IJSRuntime JS { get; set; }
        [Parameter]
        public HttpClient Http { get; set; }

        private MailDTO emailRequest = new MailDTO();
        private async Task Envoyer()
        {
            if (emailRequest == null || string.IsNullOrWhiteSpace(emailRequest.Name) || string.IsNullOrWhiteSpace(emailRequest.To)
    || string.IsNullOrWhiteSpace(emailRequest.Body))
            {
                //await JS.InvokeVoidAsync("alert", "Veuillez remplir tous les champs");
                return;
            }

            try
            {
                var response = await Http.PostAsJsonAsync("api/mail/sendgrid", emailRequest);
                MailResponseDTO? result = null;
                try
                {
                    result = await response.Content.ReadFromJsonAsync<MailResponseDTO>();
                }
                catch
                {
                    var text = await response.Content.ReadAsStringAsync();
                    result = new MailResponseDTO
                    {
                        Message = string.IsNullOrWhiteSpace(text) ? "Erreur inconnue" : text
                    };
                }

                if (response.IsSuccessStatusCode)
                {
                    //await JS.InvokeVoidAsync("alert", "Message envoyé avec succès !");
                    emailRequest = new MailDTO(); // Reset form
                    StateHasChanged();
                }
                else
                {
                    await JS.InvokeVoidAsync("alert", "Erreur : " + (result?.Message ?? "Erreur inconnue"));
                    if (!string.IsNullOrEmpty(result?.Error))
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                //await JS.InvokeVoidAsync("alert", "Erreur technique : " + ex.Message);
            }
        }

        private class MailResponseDTO
        {
            public string Message { get; set; } = "";
            public string? Error { get; set; }
        }
    }
}
