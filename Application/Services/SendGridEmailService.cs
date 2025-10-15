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
            _settings.SenderEmail ??= Environment.GetEnvironmentVariable("SENDGRID_SENDEREMAIL");
            _settings.SenderName ??= Environment.GetEnvironmentVariable("SENDGRID_SENDERNAME");
        }
        

    /*public async Task SendEmailAsync(string to, string subject, string body)
        {
            var client = new SendGridClient(_settings.ApiKey);
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
    }*/
        public async Task SendEmailAsync(string visitorEmail, string visitorName, string subject, string body)
        {
            Console.WriteLine("Démarrage de l'envoi d'email via SendGrid...");

            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            Console.WriteLine($"SENDGRID_API_KEY trouvé ? {(!string.IsNullOrEmpty(apiKey))}");
            if (string.IsNullOrEmpty(apiKey))
                throw new Exception("Clé SendGrid manquante : définis SENDGRID_API_KEY dans les variables d'environnement.");
            else
                Console.WriteLine("Clé SendGrid trouvée.");

            var client = new SendGridClient(apiKey);

            // --- Email vers moi ---
            var fromMe = new EmailAddress(_settings.SenderEmail, _settings.SenderName);
            var toMe = new EmailAddress(_settings.SenderEmail, _settings.SenderName);

            var plainTextToMe = $"Message de {visitorName} <{visitorEmail}> :\n\n{body}";
            var htmlToMe = $"<strong>Message de {visitorName} &lt;{visitorEmail}&gt; :</strong><br>{body.Replace("\n","<br>")}";

            var msgToMe = MailHelper.CreateSingleEmail(fromMe, toMe, subject, plainTextToMe, htmlToMe);
            var responseToMe = await client.SendEmailAsync(msgToMe);

            if (!responseToMe.IsSuccessStatusCode)
            {
                var text = await responseToMe.Body.ReadAsStringAsync();
                throw new Exception($"Erreur SendGrid (vers moi) : {text}");
            }

            // --- Accusé de réception vers le visiteur ---
            var fromVisitor = new EmailAddress(_settings.SenderEmail, _settings.SenderName);
            var toVisitor = new EmailAddress(visitorEmail, visitorName);

            var ackSubject = "Confirmation de réception de votre message";
            var ackBody = $"Bonjour {visitorName},\n\nMerci pour votre message. Je reviendrai vers vous rapidement.\n\nCordialement,\n{_settings.SenderName}";
            var ackHtml = $"Bonjour {visitorName},<br><br>Merci pour votre message. Je reviendrai vers vous rapidement.<br><br>Cordialement,<br>{_settings.SenderName}";

            var msgToVisitor = MailHelper.CreateSingleEmail(fromVisitor, toVisitor, ackSubject, ackBody, ackHtml);
            var responseToVisitor = await client.SendEmailAsync(msgToVisitor);

            if (!responseToVisitor.IsSuccessStatusCode)
            {
                var text = await responseToVisitor.Body.ReadAsStringAsync();
                throw new Exception($"Erreur SendGrid (vers visiteur) : {text}");
            }

            Console.WriteLine("Emails envoyés avec succès !");
        }

    }
}

