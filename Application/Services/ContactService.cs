using System.Collections.Generic;
using PortofolioApi.Domain.Entities;
using PortofolioApi.Domain.DTOs;
using PortofolioApi.Domain.Interfaces;
namespace PortofolioApi.Application.Services;

public class ContactService
{
    private readonly IRepository<Contact> _repository;
    
    public ContactService(IRepository<Contact> repository)
    {
        _repository = repository;
    }

    public IEnumerable<ContactDTO> GetContact()
    {
        var contacts = _repository.GetAll();
        List<ContactDTO> contactDto = new List<ContactDTO>();
        foreach(var contact in contacts)
        {
            contactDto.Add(new ContactDTO{
                TypeContact = contact.TypeContact,
                AdresseContact = contact.AdresseContact
            });
        }
        return contactDto; 
    }

    public ContactDTO GetContactById(int idcontact)
    {
        var contact = _repository.GetById(idcontact);
        ContactDTO contactDto = new ContactDTO{
            TypeContact = contact.TypeContact,
            AdresseContact = contact.AdresseContact
        };
        return contactDto;
    }

    public void AddContact(ContactDTO contactDTO)
    {
        Contact contact = new Contact
        {
            TypeContact = contactDTO.TypeContact,
            AdresseContact = contactDTO.AdresseContact,
            Utilisateur = new Utilisateur {
               Nom = contactDTO.Utilisateur.Nom,
               Prenom = contactDTO.Utilisateur.Prenom,
               APropos = contactDTO.Utilisateur.APropos, 
            }
        };
        _repository.Add(contact);
    }

    public void UpdateContact(ContactDTO contactDTO)
    {
        Contact contact = new Contact
        {
            TypeContact = contactDTO.TypeContact,
            AdresseContact = contactDTO.AdresseContact,
            Utilisateur = new Utilisateur {
               Nom = contactDTO.Utilisateur.Nom,
               Prenom = contactDTO.Utilisateur.Prenom,
               APropos = contactDTO.Utilisateur.APropos, 
            }
        };
        _repository.Update(contact);
    }

    public void RemoveContact(int idcontact)
    {
        _repository.Remove(idcontact);
    }

}
