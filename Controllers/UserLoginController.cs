using Microsoft.AspNetCore.Mvc;
using PortofolioApi.Domain.DTOs;
using PortofolioApi.Application.Services;
using PortofolioApi.Configuration;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;


namespace PortofolioApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserLoginController : ControllerBase
{
    private readonly UserLoginService _service;
    private readonly ITokenService _tokenservice;
    public UserLoginController(UserLoginService service, ITokenService tokenservice)
    {
        _service = service;
        _tokenservice = tokenservice;
    }

    [HttpGet]
    public IEnumerable<UserLoginResponseDTO> Get()
    //public IActionResult Get()
    {
        return _service.GetUserLogin();
    }

    /*[HttpGet("{idUserLogin}")]
    public ActionResult<UserLogin> GetById(int idUserLogin)
    {
        UserLogin userloginById = _service.GetUserLoginById(idUserLogin);
        if (userloginById == null) return NotFound();
        return userloginById;
    } */

    [HttpPost]
    public IActionResult Add([FromBody] UserLoginRequestDTO userlogin)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        _service.AddUserLogin(userlogin);
        return Ok();
    }

    [HttpPost("authenticate")]
    public async Task<ActionResult<AuthResponseDto>> Authenticate([FromBody] UserLoginRequestDTO userlogin)
    {
        var user = _service.Authenticate(userlogin.Pseudo, userlogin.MotDePasse);
        if (user == null)
            return Unauthorized(new { message = "Pseudo or password is incorrect" });
        var token = _tokenservice.CreateToken(user.IdUserLogin, user.Pseudo, user.Role);
        var settings = HttpContext.RequestServices.GetRequiredService<IOptions<JwtSettings>>().Value;
        var expiresAt = DateTime.UtcNow.AddMinutes(settings.ExpiryMinutes);

        var response = new AuthResponseDto { Token = token, ExpiresAt = expiresAt, Role = user.Role };
        //var test = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        Console.WriteLine($"Utilisateur connecté: {response.Role}, Rôle: {user.Role}");
        return Ok(response);
    }

   
    /*[HttpPut("{idUserLogin}")]
    public IActionResult Update(int idUserLogin, [FromBody] UserLogin userLogin)
    {
        UserLogin userLoginById = _service.GetUserLoginById(idUserLogin);
        if (userLoginById == null) return NotFound();
        _service.UpdateUserLogin(userLogin);
        return Ok();
    }

    [HttpDelete("{IdUserLogin}")]
    public IActionResult Delete(int idUserLogin, [FromBody] UserLogin userLogin)
    {
        UserLogin userLoginById = _service.GetUserLoginById(idUserLogin);
        if (userLoginById == null) return NotFound();
        _service.RemoveUserLogin(idUserLogin);
        return Ok();
    }*/
}
