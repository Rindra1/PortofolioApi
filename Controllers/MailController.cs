using Microsoft.AspNetCore.Mvc;
using PortofolioApi.Domain.Entities;
using PortofolioApi.Domain.DTOs;
using PortofolioApi.Application.Services;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using PortofolioApi.Services;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace PortofolioApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MailController : ControllerBase
{
    private readonly IConfiguration _config;

    public MailController(IConfiguration config)
    {
        _config = config;
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