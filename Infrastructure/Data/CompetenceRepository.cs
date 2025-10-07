using System;
using System.Collections.Generic;
using PortofolioApi.Domain.Entities;
using PortofolioApi.Domain.DTOs;
using PortofolioApi.Domain.Interfaces;
using PortofolioApi.Services;

namespace PortofolioApi.Infrastructure.Data;

public class CompetenceRepository : IRepository<Competence>
{
    private readonly ApplicationDbContext _db;
    private readonly TokenServices _tokenServices;
    public CompetenceRepository(ApplicationDbContext db, TokenServices tokenServices)
    {
        _db = db;
        _tokenServices = tokenServices;
    }

    public IEnumerable<Competence> GetAll() => _db.Competence.ToList();
    public Competence GetById(int Id) => _db.Competence.Find(Id);
    public int Add(Competence competence)
    {
        int IdUserLogin = Convert.ToInt32(_tokenServices.GetUserId());
        var utilisateur = _db.Utilisateur.FirstOrDefault(u=>u.IdUserLogin==IdUserLogin);
        if (utilisateur != null)
        {
            competence.IdUser = utilisateur.IdUser;
        }
        _db.Competence.Add(competence);
        _db.SaveChanges();
        return competence.IdCompetence;
    }

    public void Remove(int id)
    { 
        Competence? competence = _db.Competence.Find(id);
        if (competence != null)
        {
            _db.Competence.Remove(competence);
            _db.SaveChanges();
        }
    }

    public void Update(Competence competence)
    { 
        _db.Competence.Update(competence);
        _db.SaveChanges();

    }
}
