using System;
using System.Collections.Generic;
using PortofolioApi.Domain.Entities;
using PortofolioApi.Domain.Interfaces;

namespace PortofolioApi.Infrastructure.Data;

public class ContactRepository : IRepository<Contact>
{

    private readonly ApplicationDbContext _dbContext;
    public ContactRepository(ApplicationDbContext dbcontext)
    {
        _dbContext = dbcontext;
    }
    public IEnumerable<Contact> GetAll() => _dbContext.Contact.ToList();

    public Contact GetById(int id) => _dbContext.Contact.Find(id) ?? new Contact();

    public int Add(Contact contact)
    {
        var utilisateur = _dbContext.Utilisateur.FirstOrDefault(user=> user.Nom == contact.Utilisateur.Nom && user.Prenom == contact.Utilisateur.Prenom);
        contact.IdUser = utilisateur.IdUser;
        Contact? Verifiercontact = _dbContext.Contact.Find(contact.IdContact);
        if (Verifiercontact == null)
            _dbContext.Contact.Add(contact);
        else{
            contact.IdContact = Verifiercontact.IdContact;
            _dbContext.Contact.Update(contact);
        }
            
        _dbContext.SaveChanges();
        return contact.IdContact;
    }

    public void Remove(int id)
    { 
        Contact? contact = _dbContext.Contact.Find(id);
        if (contact != null)
        {
            _dbContext.Contact.Remove(contact);
            _dbContext.SaveChanges();
        }
    }

    public void Update(Contact contact)
    { 
        _dbContext.Contact.Update(contact);
        _dbContext.SaveChangesAsync();

    }

}
