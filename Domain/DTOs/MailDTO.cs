namespace PortofolioApi.Domain.DTOs;

public class MailDTO
{
    public string Name { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}
