namespace PortofolioApi.Domain.DTOs;

public class UtilisateurDTO
{
    public string resume { get; set; } = string.Empty;
    public string Nom { get; set; }
    public string Prenom { get; set; }
    public string APropos { get; set; }
    public string UserImage { get; set; }
    public List<ProjetDTO>? Projets { get; set; } = new List<ProjetDTO>();
    public List<ContactDTO>? Contacts { get; set; } = new List<ContactDTO>();
    public List<ExperienceDTO>? Experiences { get; set; } = new List<ExperienceDTO>();
    public List<CompetenceDTO>? Competences { get; set; } = new List<CompetenceDTO>();
}
