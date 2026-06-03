namespace PortofolioApi.Domain.DTOs;

public class ContactDTO
{
    public int? IdContact { get; set; }
    public String TypeContact{get;set;}
    public String AdresseContact{get;set;}
    public UtilisateurDTO? Utilisateur{get;set;}
    public int? IdUser { get; set; }
}
