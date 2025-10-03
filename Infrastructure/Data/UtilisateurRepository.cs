using System;
using System.Collections.Generic;
using PortofolioApi.Domain.Entities;
using PortofolioApi.Domain.DTOs;
using PortofolioApi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace PortofolioApi.Infrastructure.Data;
public class UtilisateurRepository : IRepository<Utilisateur>
{
    private readonly ApplicationDbContext _db;
    public UtilisateurRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    
    public IEnumerable<Utilisateur> GetAll()
    {

        Console.WriteLine("Repository Get All");
        return _db.Utilisateur.ToList();
        /*return _db.Utilisateur
                .Include(u => u.Projets)   // <-- si tu veux aussi les contacts
                .Include(ul=>ul.UserLogin)
                .Include(l=>l.Contacts)
                .ToList();*/
    } //=> _db.Utilisateur.ToList();

    public Utilisateur GetById(int id){
       Utilisateur? user = _db.Utilisateur
                        //.Include(u => u.Projets)   // <-- si tu veux aussi les contacts
                        .Include(ul=>ul.UserLogin)
                        .FirstOrDefault(u => u.IdUser == id);
        Console.WriteLine("Repository Get By Id");
        /*Utilisateur user = _db.Utilisateur
                            .Where(u=>u.IdUser == id)
                            .Select( u => new Utilisateur
                            {
                                IdUser = u.IdUser,
                                IdUserLogin = u.IdUserLogin,
                                Prenom = u.Prenom,
                                Nom = u.Nom,
                                APropos = u.APropos,
                                UserImage = u.UserImage,
                                Projets = _db.Projet
                                            .Where(p=>p.IdUser==u.IdUser)
                                            .ToList()
                            }).FirstOrDefault();*/
                            user = _db.Utilisateur
    .Include(u => u.UserLogin)
    .FirstOrDefault(u => u.IdUser == 1);

       return user?? new Utilisateur();
    } 

    public int Add(Utilisateur utilisateur)
    { 
        Console.WriteLine("Repository");
        Utilisateur? utilisateurAjouter = _db.Utilisateur.Find(utilisateur.IdUser);
        if (utilisateurAjouter != null)
            _db.Utilisateur.Update(utilisateur);
        else{
            utilisateur.IdUserLogin = utilisateur.IdUserLogin;
            _db.Utilisateur.Add(utilisateur);
        }
            
        _db.SaveChanges();
        return utilisateur.IdUser;
        
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

    public void Update(Utilisateur utilisateur)
    { 
        _db.Utilisateur.Update(utilisateur);
        _db.SaveChanges();
    
    }
}
