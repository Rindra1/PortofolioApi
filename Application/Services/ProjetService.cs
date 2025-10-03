using System.Collections.Generic;
using PortofolioApi.Domain.Entities;
using PortofolioApi.Domain.Interfaces;
namespace PortofolioApi.Application.Services;
public class ProjetService
{
    private readonly IRepository<Projet> _repository;
    public ProjetService(IRepository<Projet> repository)
    {
        _repository = repository;
    }

    public IEnumerable<Projet> GetProjet()
    {
        return _repository.GetAll();
    }

    public Projet GetProjetById(int idprojet)
    {
        return _repository.GetById(idprojet);
    }

    public int AddProjet(Projet projet)
    {
        Console.WriteLine("Projet Services");
        return _repository.Add(projet);
    }

    public void UpdateProjet(Projet projet)
    {
        _repository.Update(projet);
    }

    public void RemoveProjet(int idprojet)
    {
        _repository.Remove(idprojet);
    }
}
