using System.Linq;

namespace PortofolioApi.Domain.DTOs;

public class ProjetDTO
{
    public int? IdProjet { get; set; }
    public string? ResumerProjet { get; set; }
    public string TitreProjet { get; set; }
    public string DetailProjet { get; set; }
    public string ImageProjet { get; set; }
    public string? ImageProjet1 { get; set; }
    public string? ImageProjet2 { get; set; }
    public string? Stack { get; set; }
    public string? Lien { get; set; }
    public string? Fonctionnalite { get; set; }
    public List<LienDTO>? Liens {get;set;} = new List<LienDTO>(); 
}
