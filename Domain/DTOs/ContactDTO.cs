namespace PortofolioApi.Domain.DTOs;

public class ContactDTO
{
    public String TypeContact{get;set;}
    public String AdresseContact{get;set;}
    public UtilisateurDTO? Utilisateur{get;set;}
}
