using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PortofolioApi.Configuration;
namespace PortofolioApi.Application.Services;
public interface ITokenService
{
    string CreateToken(int userId, string username, string role);
}

public class TokenService : ITokenService
{  private readonly JwtSettings _settings;
    private readonly byte[] _key;
    private string _token;

    public TokenService(IOptions<JwtSettings> options)
    {
        _settings = options.Value;
        _key = Encoding.UTF8.GetBytes(_settings.Secret);
    }

    public string CreateToken(int userId, string username, string role)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim("role",role),
            new Claim("id", userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var creds = new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256);

        var jwttoken = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(jwttoken);
    }
    public void SetToken(string token)
    {
        _token = token;
    }

    public string GetRole()
    {
        if (string.IsNullOrEmpty(_token))
            return "";

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(_token);

        return jwt.Claims.FirstOrDefault(x => x.Type == "role")?.Value ?? "";
    }
}

