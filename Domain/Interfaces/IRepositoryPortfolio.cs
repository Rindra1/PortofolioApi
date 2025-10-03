using System.Collections.Generic;
namespace PortofolioApi.Domain.Interfaces;

public interface IRepositoryPortfolio<T>
{
    T GetPortfolioByLastName(string Prenom);
    T GetPortfolio();
}
