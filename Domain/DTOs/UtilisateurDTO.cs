namespace PortofolioApi.Domain.DTOs;

public class UtilisateurDTO
{
    public string resume { get; set; } = string.Empty;
    public string Nom { get; set; }
    public string Prenom { get; set; }
    public string APropos { get; set; }
    public string UserImage { get; set; }
    public List<ProjetDTO>? ProjetDTOs { get; set; } = new List<ProjetDTO>();
    public List<ContactDTO>? ContactDTOs { get; set; } = new List<ContactDTO>();
    public List<ExperienceDTO>? ExperienceDTOs { get; set; } = new List<ExperienceDTO>();
}
