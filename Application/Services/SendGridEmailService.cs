using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Options;
namespace PortofolioApi.Services;

public class SendGridSettings
{
    public string ApiKey { get; set; }
    public string SenderEmail { get; set; }
    public string SenderName { get; set; }
    
}

public class SendGridEmailService
{
    private readonly SendGridSettings _settings;
    public SendGridEmailService(IOptions<SendGridSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
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
}
