namespace PortofolioApi.Domain.Entities;

public class Lien
{
    public int IdLien{get;set;}
    public String? CheminLien{get;set;}
    public int IdProjet{get;set;}
    public Projet? Projet{get;set;} = new Projet();

}
