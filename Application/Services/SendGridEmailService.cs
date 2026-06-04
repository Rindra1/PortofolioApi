using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Options;
//using System.Net;
//using System.Net.Mail;

using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;


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
       /* public async Task SendEmailAsync(string visitorEmail, string visitorName, string subject, string body)
        {
            /*Console.WriteLine("Démarrage de l'envoi d'email via SendGrid...");

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
            }*/

            /*using var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(
                    Environment.GetEnvironmentVariable("SENDGRID_SENDEREMAIL"),
                    Environment.GetEnvironmentVariable("SENDMAIL_KEY")
                ),
                EnableSsl = true,
                Timeout = 15000,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };*/
            /*var smtp = new SmtpClient("smtp-relay.brevo.com", 587)
{
    Credentials = new NetworkCredential(
        "",
        ""
    ),
    EnableSsl = true,
    UseDefaultCredentials = false,
    DeliveryMethod = SmtpDeliveryMethod.Network
};

var mailToYou = new MailMessage
{
    From = new MailAddress("", "Portfolio"),
    Subject = "Nouveau message portfolio",
    Body = $"Nom: {visitorName}\nEmail: {visitorEmail}\nMessage:\n{body}"
};

mailToYou.To.Add("rindraniaina.manda@gmail.com");

var mailToVisitor = new MailMessage
{
    From = new MailAddress("ad73a2001@smtp-brevo.com", "Portfolio"),
    Subject = "Message bien reçu 👍",
    Body = $"Bonjour {visitorName}, merci pour votre message"
};

mailToVisitor.To.Add(visitorEmail);

await smtp.SendMailAsync(mailToYou);
await smtp.SendMailAsync(mailToVisitor);
            Console.WriteLine();
            Console.WriteLine("Emails envoyés avec succès !");
            Console.WriteLine();
        }

    }*/

/*public async Task SendEmailAsync(string visitorEmail, string visitorName, string visitorMessage, string subject)
{
    var gmailAddress = Environment.GetEnvironmentVariable("SENDGRID_SENDEREMAIL");
    var appPassword = Environment.GetEnvironmentVariable("SENDMAIL_KEY"); // Ton mot de passe d'application (16 caractères)
    
    using var client = new SmtpClient();
    
    // Connexion à Gmail
    await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
    await client.AuthenticateAsync(gmailAddress, appPassword);
    
    // ==========================================
    // EMAIL POUR VOUS (notification)
    // ==========================================
    var emailToYou = new MimeMessage();
    emailToYou.From.Add(new MailboxAddress("Portfolio", gmailAddress));
    emailToYou.To.Add(new MailboxAddress("Admin", gmailAddress)); // Ou votre email pro
    emailToYou.Subject = "📩 Nouveau message de votre portfolio";
    emailToYou.Body = new TextPart("plain") 
    { 
        Text = $@"
        Nouveau message reçu !

        👤 Nom : {visitorName}
        📧 Email : {visitorEmail}
        💬 Message :
        {visitorMessage}

        --- 
        Envoyé depuis votre formulaire de contact"
    };
    
    // ==========================================
    // EMAIL AU VISITEUR (accusé réception)
    // ==========================================
    var emailToVisitor = new MimeMessage();
    emailToVisitor.From.Add(new MailboxAddress("Portfolio", gmailAddress));
    emailToVisitor.To.Add(new MailboxAddress(visitorName, visitorEmail));
    emailToVisitor.Subject = "👍 Message bien reçu !";
    emailToVisitor.Body = new TextPart("plain") 
    { 
        Text = $@"
        Bonjour {visitorName},

        Merci d'avoir pris le temps de me contacter.

        J'ai bien reçu votre message et je vous répondrai dans les plus brefs délais (généralement sous 24-48h).

        En attendant, n'hésitez pas à consulter mon portfolio pour découvrir mes autres réalisations.

        Cordialement,
        Rindra Niaina RAZAFIMANDANONA

        ---
        Ceci est un message automatique, merci de ne pas y répondre directement."
    };
    
    // ==========================================
    // 3️⃣ ENVOI DES DEUX EMAILS
    // ==========================================
    await client.SendAsync(emailToYou);
    await client.SendAsync(emailToVisitor);
    
    await client.DisconnectAsync(true);
    
    Console.WriteLine($"✅ Emails envoyés : notification à vous + confirmation à {visitorEmail}");
    }*/
    

public async System.Threading.Tasks.Task SendEmailAsync(string visitorEmail, string visitorName, string subject, string visitorMessage)
{
    // Configuration avec ta clé API
    var apiInstance = new TransactionalEmailsApi();
    var apiKey = Environment.GetEnvironmentVariable("BREVO_API_KEY"); // Ta clé API Brevo
    if (!sib_api_v3_sdk.Client.Configuration.Default.ApiKey.ContainsKey("api-key"))
    {
        sib_api_v3_sdk.Client.Configuration.Default.ApiKey.Add("api-key", apiKey);
    }
    
    // Adresse expéditeur (doit être vérifiée dans Brevo)
    string senderEmail = Environment.GetEnvironmentVariable("SENDGRID_SENDEREMAIL");
    string senderName = "Portfolio";
    var formattedMessage = string.IsNullOrEmpty(visitorMessage) 
    ? "Message vide" 
    : System.Net.WebUtility.HtmlEncode(visitorMessage)  // Évite les injections HTML
        .Replace("\r\n", "<br/>")   // Retours à la ligne Windows
        .Replace("\n", "<br/>")     // Retours à la ligne Unix/Mac
        .Replace("\r", "<br/>");    // Retours à la ligne ancien Mac
    // 1️⃣ EMAIL POUR TOI (notification)
    var emailToYou = new SendSmtpEmail
    {
        To = new List<SendSmtpEmailTo>
        {
            new SendSmtpEmailTo(Environment.GetEnvironmentVariable("SENDGRID_SENDEREMAIL"), "Admin")
        },
        Sender = new SendSmtpEmailSender(senderName, senderEmail),
        Subject = "📩 Nouveau message portfolio",
        HtmlContent = $@"
            <html>
            <body>
                <h3>Nouveau message</h3>
                <p><strong>Nom :</strong> {visitorName}</p>
                <p><strong>Email :</strong> {visitorEmail}</p>
                <p><strong>Message :</strong></p>
                <div style='background-color: #f5f5f5; padding: 15px; border-radius: 5px; margin-top: 10px;'>
                {formattedMessage}
            </div>
            <hr/>
            <p style='font-size: 12px; color: #666;'>
                Vous pouvez répondre directement à cet email pour contacter {System.Net.WebUtility.HtmlEncode(visitorName)}.
            </p>
            </body>
            </html>"
    };
    

    // 2️⃣ EMAIL AU VISITEUR (confirmation)
    var emailToVisitor = new SendSmtpEmail
    {
        To = new List<SendSmtpEmailTo>
        {
            new SendSmtpEmailTo(visitorEmail, visitorName)
        },
        Sender = new SendSmtpEmailSender(senderName, senderEmail),
        Subject = "👍 Message bien reçu",
        HtmlContent = $@"
            <html>
            <body>
                <h2>Bonjour {visitorName},</h2>
                
                <p>Merci d'avoir pris le temps de me contacter.</p>

                <p>J'ai bien reçu votre message et je vous répondrai dans les plus brefs délais (généralement sous 24-48h).</p>
                <p>En attendant, n'hésitez pas à consulter mon portfolio pour découvrir mes autres réalisations.</p>

                <p>Cordialement,</p>
                <p>Rindra Niaina RAZAFIMANDANONA</p>

                ---
                <p>Ceci est un message automatique, merci de ne pas y répondre directement.</p>
            </body>
            </html>"
    };
    
    // Envoi des deux emails
    await apiInstance.SendTransacEmailAsync(emailToYou);
    await apiInstance.SendTransacEmailAsync(emailToVisitor);
    
    Console.WriteLine("✅ Emails envoyés via API Brevo");
}
    }
}

