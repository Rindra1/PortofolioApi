using System;
using System.Collections.Generic;
using PortofolioApi.Domain.Entities;
using PortofolioApi.Domain.Interfaces;

namespace PortofolioApi.Infrastructure.Data;

public class LienRepository : IRepository<Lien>
{
    private readonly ApplicationDbContext _db;
    public LienRepository(ApplicationDbContext db)
    {
        _db = db;
    }
    public IEnumerable<Lien> GetAll() => _db.Lien.ToList();

    public Lien GetById(int id) => _db.Lien.Find(id) ?? new Lien(); 
    

    public int Add(Lien lien)
    {
        Console.WriteLine("Lien Repository");
        Lien? lienAjouter = _db.Lien.Find(lien.IdLien);
        if (lienAjouter == null)
            _db.Lien.Add(lien);
        else
            _db.Lien.Update(lien);
        _db.SaveChanges();
        return lien.IdLien;
    }

    public void Remove(int id)
    {
        Lien? LienASupprimer = _db.Lien.Find(id);
        if (LienASupprimer != null)
        {
            _db.Lien.Remove(LienASupprimer);
            _db.SaveChanges();
        }
    }

    public void Update(Lien lien)
    { 
        _db.Lien.Update(lien);
        _db.SaveChanges();
    }
}
