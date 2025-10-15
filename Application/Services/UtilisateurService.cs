using System.Collections.Generic;
using PortofolioApi.Domain.Entities;
using PortofolioApi.Domain.Interfaces;
using PortofolioApi.Domain.DTOs;
namespace PortofolioApi.Application.Services;

public class UtilisateurService
{
    private readonly IRepository<Utilisateur> _repository;
    private readonly IRepositoryPortfolio<UtilisateurDTO> _repositoryPortfolio;
    public UtilisateurService(IRepository<Utilisateur> repository,IRepositoryPortfolio<UtilisateurDTO> repositoryPortfolio)
    {
        _repository = repository;
        _repositoryPortfolio = repositoryPortfolio;
    }

    public IEnumerable<UtilisateurDTO> GetUtilisateur()
    {
        var listeUtilisateur = _repository.GetAll();
        List<UtilisateurDTO> udto = new List<UtilisateurDTO>(); 
        foreach(var utilisateur in listeUtilisateur){
            udto.Add(
                new UtilisateurDTO{
                    Nom = utilisateur.Nom,
                    Prenom = utilisateur.Prenom,
                    APropos = utilisateur.APropos,
                    UserImage = utilisateur.UserImage,
                    Contacts = utilisateur.Contacts.Select(
                        c=>new ContactDTO{
                            TypeContact = c.TypeContact,
                            AdresseContact = c.AdresseContact
                        }).ToList()
            });
        }
        return udto;
    }

    public Utilisateur GetUtilisateurById(int idUtilisateur)
    {
        return _repository.GetById(idUtilisateur);
    }

    public int AddUtilisateur(Utilisateur utilisateur)
    {
        
        return _repository.Add(utilisateur);
    }

    public void UpdateUtilisateur(Utilisateur utilisateur)
    {
        _repository.Update(utilisateur);
    }

    public void RemoveUtilisateur(int idUtilisateur)
    {
        _repository.Remove(idUtilisateur);
    }
}
