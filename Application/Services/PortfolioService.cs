using System.Collections.Generic;
using PortofolioApi.Domain.Entities;
using PortofolioApi.Domain.DTOs;
using PortofolioApi.Domain.Interfaces;
namespace PortofolioApi.Application.Services;
public class PortfolioService
{
    private readonly IRepositoryPortfolio<UtilisateurDTO> _repository;
    public PortfolioService(IRepositoryPortfolio<UtilisateurDTO> repository)
    {
        _repository = repository;
    }

    public UtilisateurDTO GetPortfolio() => _repository.GetPortfolio();
}
