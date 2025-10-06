using System;
using System.Collections.Generic;
using PortofolioApi.Domain.Entities;
using PortofolioApi.Domain.DTOs;
using PortofolioApi.Domain.Interfaces;
using PortofolioApi.Services;

namespace PortofolioApi.Infrastructure.Data;

public class ExperienceRepository : IRepository<Experience>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly TokenServices _tokenServices;
    public ExperienceRepository(ApplicationDbContext dbcontext, TokenServices tokenServices)
    {
        _dbContext = dbcontext;
        _tokenServices = tokenServices;
    }
    public IEnumerable<Experience> GetAll() 
    {
        return _dbContext.Experience.ToList();
        /*.Select(e => new ExperienceDTO
        {
            TitreExperience = e.TitreExperience,
            DetailExperience = e.DetailExperience,
            DateDebut = e.DateDebut,
            DateFin = e.DateFin
        })
        .ToList();*/
    }

    public Experience GetById(int id)
    {
        return _dbContext.Experience.Find(id) ?? new Experience();
    }

      

    public int Add(Experience experience)
    {
        int IdUserLogin = Convert.ToInt32(_tokenServices.GetUserId());
        Console.WriteLine("Id User Login= " + IdUserLogin); 
        var utilisateur = _dbContext.Utilisateur.FirstOrDefault(u=>u.IdUserLogin==IdUserLogin);
        if (utilisateur != null)
        {
            experience.IdUser = utilisateur.IdUser;
        }
        _dbContext.Experience.Add(experience);
        _dbContext.SaveChanges();
        return experience.IdExperience;
    }

    public void Remove(int id)
    { 
        Experience? experience = _dbContext.Experience.Find(id);
        if (experience != null)
        {
            _dbContext.Experience.Remove(experience);
            _dbContext.SaveChanges();
        }
    }

    public void Update(Experience experience)
    { 
        _dbContext.Experience.Update(experience);
        _dbContext.SaveChanges();

    }
}
