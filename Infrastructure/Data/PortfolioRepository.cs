using System;
using System.Collections.Generic;
using PortofolioApi.Domain.Entities;
using PortofolioApi.Domain.DTOs;
using PortofolioApi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace PortofolioApi.Infrastructure.Data;

public class PortfolioRepository : IRepositoryPortfolio<UtilisateurDTO>
{
    private readonly ApplicationDbContext _db;
    public PortfolioRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public UtilisateurDTO GetPortfolio()
    {
        var utilisateurDto = _db.Utilisateur
        .AsSplitQuery() //évite les requêtes géantes avec doublons
        .OrderBy(u => u.IdUser) // ordre déterministe pour FirstOrDefault
        .Select(u => new UtilisateurDTO
        {
            resume = u.resume,
            Nom = u.Nom,
            Prenom = u.Prenom,
            APropos = u.APropos,
            UserImage = u.UserImage,

            Contacts = u.Contacts.Select(c => new ContactDTO
            {
                TypeContact = c.TypeContact,
                AdresseContact = c.AdresseContact
            }).ToList(),

            Experiences = u.Experiences.Select(e => new ExperienceDTO
            {
                IdExperience = e.IdExperience,
                TitreExperience = e.TitreExperience,
                DetailExperience = e.DetailExperience,
                DateDebut = e.DateDebut,
                DateFin = e.DateFin
            }).ToList(),

            Competences = u.Competences.Select(e => new CompetenceDTO
            {
                IdCompetence = e.IdCompetence,
                Nom = e.Nom
            }).ToList(),

            Projets = u.Projets.Select(p => new ProjetDTO
            {
                IdProjet = p.IdProjet,
                ResumerProjet = p.ResumerProjet,
                TitreProjet = p.TitreProjet,
                DetailProjet = p.DetailProjet,
                ImageProjet = p.ImageProjet,

                Liens = p.Liens.Select(t => new LienDTO
                {
                    CheminLien = t.CheminLien
                }).ToList()
            }).ToList()
        })
        .FirstOrDefault(); 

        return utilisateurDto ?? new UtilisateurDTO();
    }



    public UtilisateurDTO GetPortfolioByLastName(string Prenom)
    {
        var utilisateurDto = _db.Utilisateur
            .Where(u => u.Prenom == Prenom) // filtre pour un seul utilisateur
            .Select(u => new UtilisateurDTO
            {
                Nom = u.Nom,
                Prenom = u.Prenom,
                APropos = u.APropos,
                UserImage = u.UserImage,
                Contacts = u.Contacts.Select(c => new ContactDTO
                {
                    TypeContact = c.TypeContact,
                    AdresseContact = c.AdresseContact
                }).ToList(),
                Projets = u.Projets.Select(p => new ProjetDTO
                {
                    TitreProjet = p.TitreProjet,
                    DetailProjet = p.DetailProjet,
                    ImageProjet = p.ImageProjet,

                    Liens = p.Liens.Select(t => new LienDTO
                    {
                        CheminLien = t.CheminLien
                    }).ToList()
                }).ToList()
            })
        .FirstOrDefault(); // retourne un seul DTO
        return utilisateurDto ?? new UtilisateurDTO();
    }
}
