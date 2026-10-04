using Microsoft.AspNetCore.Mvc;
using PortofolioApi.Domain.Entities;
using PortofolioApi.Domain.DTOs;
using PortofolioApi.Services;
using PortofolioApi.Application.Services;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PortofolioApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PortfolioController : ControllerBase
{
    private readonly PortfolioService _service;
    public PortfolioController(PortfolioService service)
    {
        _service = service;
    }

    [HttpGet]
    [HttpGet]
    public ActionResult<UtilisateurDTO> GetPortfolio()
    {
        try
        {
            var portfolio = _service.GetPortfolio();

            return Ok(portfolio);
        }
        catch (Exception ex)
        {
            Console.WriteLine("========== ERREUR PORTFOLIO ==========");
            Console.WriteLine(ex.ToString());
            Console.WriteLine("=======================================");

            return StatusCode(500, new
            {
                message = ex.Message,
                detail = ex.InnerException?.Message
            });
        }
    }

}
