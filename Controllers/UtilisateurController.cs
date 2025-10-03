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
public class UtilisateurController : ControllerBase
{
    private readonly UtilisateurService _service;
    private readonly TokenServices _tokenService;
    public UtilisateurController(UtilisateurService service, TokenServices tokenService)
    {
        _service = service;
        _tokenService = tokenService;
    }

    [HttpGet]
    public IEnumerable<UtilisateurDTO> Get()
    {
        return _service.GetUtilisateur();
    }

    [HttpGet("{idUtilisateur}")]
    public ActionResult<UtilisateurDTO> GetById(int idUtilisateur)
    {
       
        Utilisateur utilisateurById = _service.GetUtilisateurById(idUtilisateur);
        if (utilisateurById == null) return NotFound();
        return Ok(utilisateurById);
    }

    
    [HttpPost]
    [AllowAnonymous] 
    public IActionResult Add([FromBody] UtilisateurDTO utilisateurDTO)
    {
        Console.WriteLine("Utilisateur Controller");
       // Crée l'utilisateur
        var utilisateur = new Utilisateur
        {
            IdUserLogin = Convert.ToInt32(_tokenService.GetUserId()),
            Nom = utilisateurDTO.Nom,
            Prenom = utilisateurDTO.Prenom,
            APropos = utilisateurDTO.APropos,
            UserImage = utilisateurDTO.UserImage,
            resume = utilisateurDTO.resume,
            Contacts = new List<Contact>()
        };

        // Ajoute les contacts liés à cet utilisateur
        foreach (var contactDTO in utilisateurDTO.ContactDTOs)
        {
            utilisateur.Contacts.Add(new Contact
            {
                TypeContact = contactDTO.TypeContact,
                AdresseContact = contactDTO.AdresseContact
            });
            Console.WriteLine(contactDTO.TypeContact + " Tonga ato " + contactDTO.AdresseContact);
        }
        int utilisateurid = _service.AddUtilisateur(utilisateur);
        return Ok(utilisateurid);
    }

    [HttpPut("{IdUtilisateur}")]
    public IActionResult Update(int IdUtilisateur, [FromBody] Utilisateur utilisateur)
    {
        Utilisateur UtilisateurById = _service.GetUtilisateurById(IdUtilisateur);
        if (UtilisateurById == null) return NotFound();
        _service.UpdateUtilisateur(utilisateur);
        return Ok();
    }

    [HttpDelete("{idUtilisateur}")]
    public IActionResult Delete(int idUtilisateur)
    {
        Utilisateur UtilisateurById = _service.GetUtilisateurById(idUtilisateur);
        if (UtilisateurById == null) return NotFound();
        _service.RemoveUtilisateur(idUtilisateur);
        return Ok();

    }

}
