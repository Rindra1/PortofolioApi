using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Options;
namespace PortofolioApi.Application.Services;
public class SendGridEmailService
{
    private readonly SendGridSettings _settings;
    public SendGridEmailService(IOptions<SendGridSettings> settings)
    {
        _settings = settings.Value;
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
    }*/
    public async Task SendEmailAsync(string to, string subject, string body)
{
    // On lit la clé SendGrid depuis les variables d’environnement
    var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");

    if (string.IsNullOrEmpty(apiKey))
        throw new Exception("Clé SendGrid manquante : définis SENDGRID_API_KEY dans les variables d'environnement.");

    // On garde le reste identique
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

