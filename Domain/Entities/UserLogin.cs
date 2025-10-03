using System.ComponentModel.DataAnnotations;
namespace PortofolioApi.Domain.Entities;

public class UserLogin
{
    public int IdUserLogin { get; set; }
    [Required(ErrorMessage = "Le pseudo est requis")]
    public String? Pseudo { get; set; }
    [Required(ErrorMessage = "Le rôle est obligatoire")]
    public String? Role { get; set; }
    [Required(ErrorMessage = "Le mot de passe est requis")]
    [MinLength(6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères")]
    public String? MotDePasse { get; set; }
    //public int UtilisateurId { get; set; }
    //public Utilisateur? Utilisateur { get; set; }
    public virtual Utilisateur? Utilisateur {get;set;}
}
