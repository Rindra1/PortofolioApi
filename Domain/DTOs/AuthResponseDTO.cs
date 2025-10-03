namespace PortofolioApi.Domain.DTOs;

public class AuthResponseDto
{
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string Role { get; set;}
}
