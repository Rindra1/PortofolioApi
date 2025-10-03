namespace PortofolioApi.Domain.Entities;

public class Projet
{
    public int IdProjet { get; set; }
    public string? ResumerProjet { get; set; }
    public string? TitreProjet { get; set; }
    public string? DetailProjet { get; set; }
    public string? ImageProjet { get; set; }

    // Clé étrangère vers l'utilisateur
    public int UtilisateurId { get; set; }  
    public Utilisateur? Utilisateur { get; set; }

    public List<Lien> Liens { get; set; } = new List<Lien>();
}
