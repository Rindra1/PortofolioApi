namespace PortofolioApi.Domain.Entities;

public class Utilisateur
{
    public int IdUser { get; set; }
    public int? IdUserLogin { get; set; }
    public string resume { get; set; } = string.Empty;
    public string? Nom { get; set; }
    public string? Prenom { get; set; }
    public string? APropos { get; set; }
    public string? UserImage { get; set; }
    //public UserLogin? UserLogin { get; set; }
    public virtual UserLogin? UserLogin {get;set;}
    public List<Projet> Projets { get; set; } = new List<Projet>();
    public List<Contact> Contacts { get; set; } = new List<Contact>();
    public List<Experience> Experiences { get; set; } = new List<Experience>();
    public List<Competence> Competences { get; set; } = new List<Competence>();
}
