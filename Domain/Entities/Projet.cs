namespace PortofolioApi.Domain.Entities;

public class Projet
{
    public int IdProjet { get; set; }
    public string? ResumerProjet { get; set; }
    public string? TitreProjet { get; set; }
    public string? DetailProjet { get; set; }
    public string? ImageProjet { get; set; }
    public string? ImageProjet1 { get; set; }
    public string? ImageProjet2 { get; set; }
    public string? Stack { get; set; }
    public string? Lien { get; set; }
    public string? Fonctionnalite { get; set; }

    // Clé étrangère vers l'utilisateur
    public int UtilisateurId { get; set; }  
    public Utilisateur? Utilisateur { get; set; }

    public List<Lien> Liens { get; set; } = new List<Lien>();
}
