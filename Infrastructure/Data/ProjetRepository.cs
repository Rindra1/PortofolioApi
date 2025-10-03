using System;
using System.Text;
using System.Collections.Generic;
using PortofolioApi.Domain.Entities;
using PortofolioApi.Domain.Interfaces;
using PortofolioApi.Services;

namespace PortofolioApi.Infrastructure.Data;

public class ProjetRepository : IRepository<Projet>
{
    private readonly ApplicationDbContext _db;
    private readonly TokenServices _tokenServices;
    public ProjetRepository(ApplicationDbContext db, TokenServices tokenServices)
    {
        _db = db;
        _tokenServices = tokenServices;
    }
    public IEnumerable<Projet> GetAll() => _db.Projet.ToList();

    public Projet GetById(int id) => _db.Projet.Find(id) ?? new Projet();

    public int Add(Projet projet)
    {
        Console.WriteLine("Projet Repository"); 
        int IdUserLogin = Convert.ToInt32(_tokenServices.GetUserId());
        Console.WriteLine("Id User Login= " + IdUserLogin); 
        var utilisateur = _db.Utilisateur.FirstOrDefault(u=>u.IdUserLogin==IdUserLogin);
        if (utilisateur != null)
        {
            projet.UtilisateurId = utilisateur.IdUser;
        }
        Projet? projetAjouter = _db.Projet.Find(projet.IdProjet);
        if (projetAjouter != null)
        {
            projet.IdProjet = projetAjouter.UtilisateurId;
            _db.Projet.Update(projet);
        }
        else
        {
            _db.Projet.Add(projet);
        }
        _db.SaveChanges();
        return projet.IdProjet;
    }

    public void Update(Projet projet)
    {
        _db.Projet.Update(projet);
        _db.SaveChanges();
    }

    public void Remove(int id)
    { 
        Projet? ProjetASupprimer = _db.Projet.Find(id);
        if (ProjetASupprimer != null)
        {
            _db.Projet.Remove(ProjetASupprimer);
            _db.SaveChanges();
        }
    }
    

}
