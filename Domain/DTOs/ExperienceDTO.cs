namespace PortofolioApi.Domain.DTOs;

public class ExperienceDTO
{
    public int IdExperience { get; set; }
    public string TitreExperience { get; set; } = string.Empty;
    public string DetailExperience { get; set; } = string.Empty;
    public DateTime DateDebut { get; set; }
    public DateTime? DateFin { get; set; }
}
