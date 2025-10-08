using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using PortofolioApi.Application.Services;
using PortofolioApi.Domain.DTOs;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly ChatService _chatService;
    private readonly PortfolioService _portfolio;

    public ChatController(ChatService chatService, PortfolioService portfolio)
    {
        _chatService = chatService;
        _portfolio = portfolio;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ChatRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Message))
            return BadRequest(new { error = "Message manquant" });

        try
        {
            // Récupère les infos pertinentes depuis la DB
            var utilisateur = _portfolio.GetPortfolio();
            
            // Construire le prompt pour le modèle
            var prompt = "Voici les informations sur mon portfolio :\n";
            prompt += $"Nom: {utilisateur.Nom}\nPrénom: {utilisateur.Prenom}\nResume: {utilisateur.resume}\nA propos: {utilisateur.APropos}\n\n";
            foreach(var contact in utilisateur.ContactDTOs)
            {
                prompt += $"Contact : {contact.AdresseContact}\n\n";
            }
            foreach (var projet in utilisateur.ProjetDTOs)
            {
                prompt += $"Titre : {projet.TitreProjet}\nResumé : {projet.ResumerProjet}\nDétails : {projet.DetailProjet}\n\n";
            }
            foreach (var experience in utilisateur.ExperienceDTOs)
            {
                prompt += $"Titre : {experience.TitreExperience}\nDétail : {experience.DetailExperience}\nDate début : {experience.DateDebut}\nDate Fin : {experience.DateFin}\n\n";
            }
            foreach(var competence in utilisateur.CompetenceDTOs)
            {
                prompt += $"Competence : {competence.Nom}\n\n";
            }
            prompt += $"Réponds à la question suivante : \"{request.Message}\"";
            //prompt += "⚠️ Réponds le moins que possible, de manière concise et claire.";
            //prompt += "⚠️ Réponds par 300 mots maximum, de manière concise et claire.";
            prompt += "⚠️ Réponds par 300 mots maximum, de manière concise et claire, et sans utiliser de tirets ou de Markdown.";
            //var prompt = "Réponds à la question suivante : \"Bonjour\"";
            var responseMessage = await _chatService.SendMessageAsync(prompt);

            return Ok(new ChatResponse(responseMessage));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
