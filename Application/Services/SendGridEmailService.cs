using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Options;

namespace PortofolioApi.Application.Services
{
    public class SendGridEmailService
    {
        private readonly SendGridSettings _settings;

        public SendGridEmailService(IOptions<SendGridSettings> settings)
        {
            _settings = settings.Value;

            // Si les valeurs ne sont pas dans appsettings.json, on les lit dans les variables d'environnement
            _settings.SenderEmail ??= Environment.GetEnvironmentVariable("SENDGRID__SENDEREMAIL");
            _settings.SenderName ??= Environment.GetEnvironmentVariable("SENDGRID__SENDERNAME");
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID__APIKEY");

            if (string.IsNullOrEmpty(apiKey))
                throw new Exception("Clé SendGrid manquante : définis SENDGRID_API_KEY dans les variables d'environnement sur Render.");

            if (string.IsNullOrEmpty(_settings.SenderEmail))
                throw new Exception("SenderEmail non défini. Ajoute-le dans les variables d'environnement Render.");

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_settings.SenderEmail, _settings.SenderName);
            var toEmail = new EmailAddress(to);

            var msg = MailHelper.CreateSingleEmail(from, toEmail, subject, "", body);
            msg.HtmlContent = body;

            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                var text = await response.Body.ReadAsStringAsync();
                throw new Exception($"Erreur SendGrid: {text}");
            }
        }
    }
}
