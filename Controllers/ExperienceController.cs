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

public class ExperienceController : ControllerBase
{
    private readonly ExperienceService _service;
    public ExperienceController(ExperienceService service)
    {
        _service = service;
    }
    [HttpGet]
    public IEnumerable<Experience> Get()
    {

        return _service.GetExperience();
    }
    [HttpGet("{id}")]
    public Experience GetById(int id)
    {
        return _service.GetExperienceById(id);
    }
    [HttpPost]
    public IActionResult Post([FromBody] ExperienceDTO experienceDTO)
    {
        _service.AddExperience(experienceDTO);
        return Ok(new { Message = "Experience ajoutée avec succès !" });
    }

    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody] ExperienceDTO experienceDTO)
    {
        var existing = _service.GetExperienceById(id);
        if (existing == null) return NotFound();
        experienceDTO.IdExperience = id;
        _service.UpdateExperience(experienceDTO);
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var existing = _service.GetExperienceById(id);
        if (existing == null) return NotFound();
        _service.RemoveExperience(id);
        return Ok();
    }
}
