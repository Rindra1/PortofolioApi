using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

using System.Collections.Generic;
using PortofolioApi.Domain.Entities;
using PortofolioApi.Domain.Interfaces;

namespace PortofolioApi.Infrastructure.Data;

public class UserLoginRepository : IRepository<UserLogin>
{
    private readonly ApplicationDbContext _db;
    private readonly string salt = Guid.NewGuid().ToString(); // Longueur du sel en octets
    
    #region Crypter Mot De Passe
        private string CrypterMotDePasse(string mot_de_passe)
        {
            var crypte = Encoding.Unicode.GetBytes(mot_de_passe);
            return Convert.ToBase64String(crypte);
        }
        #endregion

        #region Decrypter Mot De Passe
        private string DecrypterMotDePasse(string mot_de_passe)
        {
            var decrypte = Convert.FromBase64String(mot_de_passe);
            return Encoding.Unicode.GetString(decrypte);
        }
        #endregion

    
    public UserLoginRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public IEnumerable<UserLogin> GetAll() 
    {
        var lst = _db.UserLogin.ToList();
        foreach(var l in lst){
            Console.WriteLine(l.Pseudo + " " + l.Role + " " + l.MotDePasse);
        }
        return _db.UserLogin.ToList();
    }

    public UserLogin GetById(int id){
        return _db.UserLogin.Find(id) ?? new UserLogin();
    } 

    public int Add(UserLogin userlogin)
    {
        UserLogin? UserLoginAjouter = _db.UserLogin.Find(userlogin.IdUserLogin);
        if (UserLoginAjouter != null)
        {
            //userlogin.MotDePasse = CrypterMotDePasse(userlogin.MotDePasse!);
            _db.UserLogin.Update(userlogin);
        }
        else
        {
            //userlogin.MotDePasse = CrypterMotDePasse(userlogin.MotDePasse!);
            _db.UserLogin.Add(userlogin);
        }
        _db.SaveChanges();
        return userlogin.IdUserLogin;
    }

    public void Remove(int id)
    {
        UserLogin UserLoginASupprimer = _db.UserLogin.Find(id) ?? new UserLogin();
        if (UserLoginASupprimer != null)
        {
            _db.UserLogin.Remove(UserLoginASupprimer);
            _db.SaveChanges();
        }
    }

    public void Update(UserLogin userlogin)
    {
        //userlogin.MotDePasse = CrypterMotDePasse(userlogin.MotDePasse!);
        _db.UserLogin.Update(userlogin);
        _db.SaveChanges();
    }
}
