namespace PortofolioApi.Domain.DTOs;

public class CompetenceDTO
{
    public int IdCompetence{get;set;}
    public String Nom{get;set;}
    public UtilisateurDTO? Utilisateur{get;set;}
}
