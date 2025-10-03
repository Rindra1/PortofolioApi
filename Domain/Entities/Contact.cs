namespace PortofolioApi.Domain.Entities;

public class Contact
{
    public int IdContact{get;set;}
    public String? TypeContact{get;set;}
    public String? AdresseContact{get;set;}

    public int IdUser { get; set; } // clé étrangère
    public Utilisateur? Utilisateur{get;set;}
}
