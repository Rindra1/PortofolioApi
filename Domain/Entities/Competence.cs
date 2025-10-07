namespace PortofolioApi.Domain.Entities;

public class Competence
{
    public int IdCompetence{get;set;}
    public string? Nom{get;set;} = string.Empty;
    public int IdUser { get; set; }
    public Utilisateur? Utilisateur { get; set; }
}
