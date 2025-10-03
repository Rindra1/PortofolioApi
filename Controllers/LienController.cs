using Microsoft.AspNetCore.Mvc;
using PortofolioApi.Domain.Entities;
using PortofolioApi.Domain.DTOs;
using PortofolioApi.Application.Services;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using PortofolioApi.Services;

namespace PortofolioApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LienController : ControllerBase
{
    private readonly LienService _service;
    private readonly TokenServices _tokenService;
    public LienController(LienService service, TokenServices tokenService)
    {
        _service = service;
        _tokenService = tokenService;
    }

    [HttpGet]
    public IEnumerable<Lien> Get()
    {
        return _service.GetLien();
    }

    [HttpGet("{idLien}")]
    public ActionResult<Lien> GetById(int idLien)
    {
        Lien lien = _service.GetLienById(idLien);
        if (lien == null) return NotFound();
        return Ok(lien);
    }

    [HttpPost]
    public IActionResult Add([FromBody] LienDTO lien)
    {
        Console.WriteLine("Lien Controller");
        _service.AddLien(lien);
        return Ok();
    }

    [HttpPut("{idLien}")]
    public IActionResult Update(int idLien,[FromBody] Lien lien)
    {
        Lien LienById = _service.GetLienById(idLien);
        if (LienById==null) return NotFound();
        _service.UpdateLien(lien);
        return Ok();
    }

    [HttpDelete("{idLien}")]
    public IActionResult Remove(int idLien)
    {
        Lien lienASupprimer = _service.GetLienById(idLien);
        if (lienASupprimer == null) return NotFound();
        _service.RemoveLien(idLien);
        return Ok();
    }

}
