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
        [Inject]
        public HttpClient Http { get; set; }

        private MailDTO emailRequest = new MailDTO();
        private string? formMessage;
        private bool isSending;
        private bool isSuccess;
        private string formMessageCss => isSuccess ? "form-message success" : "form-message error";

        private async Task Envoyer()
        {
            if (emailRequest == null || string.IsNullOrWhiteSpace(emailRequest.Name) || string.IsNullOrWhiteSpace(emailRequest.To)
    || string.IsNullOrWhiteSpace(emailRequest.Body))
            {
                isSuccess = false;
                formMessage = Localizer?.T("FillAllFields") ?? "Veuillez remplir tous les champs.";
                return;
            }

            isSending = true;
            formMessage = null;

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
                    emailRequest = new MailDTO(); // Reset form
                    isSuccess = true;
                    formMessage = Localizer?.T("MessageSent") ?? "Message envoyé avec succès !";
                }
                else
                {
                    isSuccess = false;
                    formMessage = result?.Message ?? "Erreur lors de l'envoi du message.";
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                formMessage = "Erreur technique : " + ex.Message;
            }
            finally
            {
                isSending = false;
            }
        }

        private class MailResponseDTO
        {
            public string Message { get; set; } = "";
            public string? Error { get; set; }
        }
    }
}
