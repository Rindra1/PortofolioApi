using System;
using System.Collections.Generic;
using PortofolioApi.Domain.Entities;
using PortofolioApi.Domain.DTOs;
using PortofolioApi.Domain.Interfaces;

namespace PortofolioApi.Infrastructure.Data;

public class ExperienceRepository : IRepository<Experience>
{
    private readonly ApplicationDbContext _dbContext;
    public ExperienceRepository(ApplicationDbContext dbcontext)
    {
        _dbContext = dbcontext;
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
        /*var experience = _dbContext.Experience
            .Where(e => e.IdExperience == id)
            .Select(e => new ExperienceDTO
            {
                IdExperience = e.IdExperience,
                TitreExperience = e.TitreExperience,
                DetailExperience = e.DetailExperience,
                DateDebut = e.DateDebut,
                DateFin = e.DateFin
            })
            .FirstOrDefault();*/

        //return experience ?? new ExperienceDTO();
        return _dbContext.Experience.Find(id) ?? new Experience();
    }

      

    public int Add(Experience experience)
    {
        
        return 1;
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
        _dbContext.SaveChangesAsync();

    }
}
