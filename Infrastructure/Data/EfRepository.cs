using PortofolioApi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using PortofolioApi.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;

namespace PortofolioApi.Infrastructure.Data
{
    public class EfRepository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public EfRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public int Add(T entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();

            // Récupère la valeur de la clé primaire avec EF Core
            var keyName = _context.Model.FindEntityType(typeof(T))
                                .FindPrimaryKey()
                                .Properties
                                .Select(x => x.Name)
                                .FirstOrDefault();

            if (keyName == null) 
                throw new Exception("Aucune clé primaire trouvée pour l'entité " + typeof(T).Name);

            // Récupère la valeur de la clé primaire dynamiquement
            var keyValue = entity.GetType().GetProperty(keyName)?.GetValue(entity, null);

            return keyValue != null ? Convert.ToInt32(keyValue) : 0;
        }


        public void Remove(int id)
        {
            var entity = _dbSet.Find(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                _context.SaveChanges();
            }
        }

        public T GetById(int id)
        {
            return _dbSet.Find(id)!;
        }

        public IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
            _context.SaveChanges();
        }
    }
}
