using System.Collections.Generic;
using PortofolioApi.Domain.Entities;
using PortofolioApi.Domain.DTOs;
using PortofolioApi.Domain.Interfaces;
namespace PortofolioApi.Application.Services;

public class LienService
{
    private readonly IRepository<Lien> _repository;
    public LienService(IRepository<Lien> repository)
    {
        _repository = repository;
    }

    public IEnumerable<Lien> GetLien()
    {
        return _repository.GetAll();
    }

    public Lien GetLienById(int idlien)
    {
        return _repository.GetById(idlien);
    }

    public void AddLien(LienDTO liendto)
    {
        Console.WriteLine("Lien Services");
        Lien lien = new Lien{
            CheminLien = liendto.CheminLien,
            IdProjet = liendto.IdProjet
        };
        _repository.Add(lien);
    }

    public void UpdateLien(Lien lien)
    {
        _repository.Update(lien);
    }

    public void RemoveLien(int idlien)
    {
        _repository.Remove(idlien);
    }
}
