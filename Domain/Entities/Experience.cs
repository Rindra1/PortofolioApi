namespace PortofolioApi.Domain.Entities;

public class Experience
{
    public int IdExperience { get; set; }
    public string TitreExperience { get; set; } = string.Empty;
    public string DetailExperience { get; set; } = string.Empty;
    public DateTime DateDebut { get; set; }
    public DateTime? DateFin { get; set; }
    public int IdUser { get; set; }
    public Utilisateur? Utilisateur { get; set; }
}
