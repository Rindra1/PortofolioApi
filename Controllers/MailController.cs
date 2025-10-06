using Microsoft.AspNetCore.Mvc;
using PortofolioApi.Domain.DTOs;
using PortofolioApi.Application.Services;
using System.Net.Mail;

using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Options;

namespace PortofolioApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MailController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly SendGridEmailService _emailService;
    private readonly SendGridSettings _settings;
    public MailController(IConfiguration config, SendGridEmailService emailService, IOptions<SendGridSettings> settings)
    {
        _config = config;
        _emailService = emailService;
        _settings = settings.Value;

        _settings.SenderEmail ??= Environment.GetEnvironmentVariable("SENDGRID_SENDEREMAIL");
        _settings.SenderName ??= Environment.GetEnvironmentVariable("SENDGRID_SENDERNAME");
        
    }

    [HttpPost("sendgrid")]
    public async Task<IActionResult> Post([FromBody] MailDTO model)
    {
        if (string.IsNullOrWhiteSpace(model.To) || !model.To.Contains("@"))
            return BadRequest(new { Message = "Adresse e-mail invalide" });
        // Envoyer le message à moi
        await _emailService.SendEmailAsync(
            _settings.SenderEmail,
            $"Nouveau message de {model.Name}",
            $"Email: {model.To}\nMessage: {model.Subject}"
        );

        //Envoyer un accusé de réception au visiteur
        await _emailService.SendEmailAsync(
            model.To,
            "Merci pour votre message !",
            $"Bonjour {model.Name},\n\nNous avons bien reçu votre message et nous vous répondrons bientôt."
        );

        //await _emailService.SendEmailAsync(model.To, model.Subject, model.Body);
        return Ok(new { Message = "E-mail envoyé avec succès !" });

    }

    [HttpPost]
    public async Task<IActionResult> SendEmail([FromBody] MailDTO emailRequest)
    {
        try
        {
            string host = _config["Smtp:Host"];
            int port = int.Parse(_config["Smtp:Port"]);
            string user = _config["Smtp:User"];
            string pass = _config["Smtp:Password"]; // App password Gmail

            using (var smtp = new SmtpClient(host, port))
            {
                smtp.EnableSsl = true; // TLS
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential(user, pass);

                using (var message = new MailMessage(user, emailRequest.To, emailRequest.Subject, emailRequest.Body))
                {
                    await smtp.SendMailAsync(message);
                }
            }

            // Réponse JSON toujours valide
            return Ok(new MailResponseDTO { Message = "Email envoyé avec succès" });
        }
        catch (SmtpException smtpEx)
        {
            return StatusCode(500, new MailResponseDTO
            {
                Message = "Erreur SMTP",
                Error = smtpEx.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new MailResponseDTO
            {
                Message = "Erreur inattendue",
                Error = ex.Message
            });
        }
    }

}