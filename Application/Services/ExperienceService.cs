using System.Collections.Generic;
using PortofolioApi.Domain.Entities;
using PortofolioApi.Domain.DTOs;
using PortofolioApi.Domain.Interfaces;
namespace PortofolioApi.Services;

public class ExperienceService
{
    private readonly IRepository<Experience> _repository;
    public ExperienceService(IRepository<Experience> repository)
    {
        _repository = repository;
    }

    public IEnumerable<Experience> GetExperience()
    {
        return _repository.GetAll(); 
    }

    public Experience GetExperienceById(int idcontact)
    {
        return _repository.GetById(idcontact);
    }

    public void AddExperience(ExperienceDTO experienceDTO)
    {
        Experience experience = new Experience
        {
            DateDebut = experienceDTO.DateDebut,
            DateFin = experienceDTO.DateFin,
            TitreExperience = experienceDTO.TitreExperience,
            DetailExperience = experienceDTO.DetailExperience        
        };

        /*Contact contact = new Contact
        {
            TypeContact = contactDTO.TypeContact,
            AdresseContact = contactDTO.AdresseContact,
            Utilisateur = new Utilisateur {
               Nom = contactDTO.Utilisateur.Nom,
               Prenom = contactDTO.Utilisateur.Prenom,
               APropos = contactDTO.Utilisateur.APropos, 
            }
        };*/
        _repository.Add(experience);
    }

    public void UpdateExperience(ExperienceDTO experienceDTO)
    {
        Experience experience = new Experience
        {
            DateDebut = experienceDTO.DateDebut,
            DateFin = experienceDTO.DateFin,
            TitreExperience = experienceDTO.TitreExperience,
            DetailExperience = experienceDTO.DetailExperience
            
        };
        _repository.Update(experience);
    }

    public void RemoveExperience(int idExperience)
    {
        _repository.Remove(idExperience);
    }
}
