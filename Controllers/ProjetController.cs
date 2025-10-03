using Microsoft.AspNetCore.Mvc;
using PortofolioApi.Domain.Entities;
using PortofolioApi.Domain.DTOs;
using PortofolioApi.Application.Services;
using PortofolioApi.Services;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PortofolioApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjetController : ControllerBase
{
    private readonly ProjetService _service;
    private readonly TokenServices _tokenService;
    public ProjetController(ProjetService service, TokenServices tokenService)
    {
        _service = service;
        _tokenService = tokenService;
    }

    [HttpGet]
    public IEnumerable<Projet> Get()
    {
        return _service.GetProjet();
    }

    [HttpGet("{idProjet}")]
    public ActionResult<Projet> GetById(int idProjet)
    {
        Projet projetById = _service.GetProjetById(idProjet);
        if (projetById == null) return NotFound();
        return Ok(projetById);
    }

    [HttpPost]
    [AllowAnonymous]
    public ActionResult<int> Add([FromBody] ProjetDTO projetdto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return BadRequest(errors);
        }

        Console.WriteLine("Projet Controller");
        Projet projet = new Projet{
            ResumerProjet = projetdto.ResumerProjet,
            TitreProjet = projetdto.TitreProjet,
            DetailProjet = projetdto.DetailProjet,
            ImageProjet = projetdto.ImageProjet
        };
        int IdProjet = _service.AddProjet(projet);
        return Ok(IdProjet);
    }

    [HttpPut("{idProjet}")]
    public IActionResult Update(int idProjet,[FromBody] Projet projet)
    {
        Projet projetById = _service.GetProjetById(idProjet);
        if (projetById == null) return NotFound();
        _service.UpdateProjet(projet);
        return Ok();
    }

    [HttpDelete("{idProjet}")]
    public IActionResult Delete(int idProjet)
    {
        Projet projetById = _service.GetProjetById(idProjet);
        if (projetById == null) return NotFound();
        _service.RemoveProjet(idProjet);
        return Ok();
    }

}
