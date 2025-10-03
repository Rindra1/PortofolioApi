using System.Collections.Generic;

namespace PortofolioApi.Domain.Interfaces;

public interface IRepository<T>
{
    int Add(T entites);
    void Remove(int id);
    T GetById(int id);
    IEnumerable<T> GetAll();
    void Update(T entities);
}
