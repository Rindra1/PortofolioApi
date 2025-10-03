using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using PortofolioApi.Domain.Entities;
using PortofolioApi.Domain.DTOs;
using PortofolioApi.Domain.Interfaces;

namespace PortofolioApi.Application.Services;

public class UserLoginService
{
    private readonly IRepository<UserLogin> _repository;
    public UserLoginService(IRepository<UserLogin> repository)
    {
        _repository = repository;
    }

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


    public IEnumerable<UserLoginResponseDTO> GetUserLogin()
    {
        var listeUserLogin = _repository.GetAll();
        List<UserLoginResponseDTO> lstResponse = new List<UserLoginResponseDTO>();
        foreach(var l in listeUserLogin)
        {
            UserLoginResponseDTO dto = new UserLoginResponseDTO{
            Pseudo = l.Pseudo,
            Role = l.Role
            };
            lstResponse.Add(dto);
        }  
        return lstResponse;
    }

    public UserLogin Authenticate(string pseudo, string motDePasse)
    {
        var user = _repository.GetAll().SingleOrDefault(x => x.Pseudo == pseudo && x.MotDePasse == CrypterMotDePasse(motDePasse));
        /*UserLoginResponseDTO dto = new UserLoginResponseDTO{
            Pseudo = user.Pseudo,
            Role = user.Role
        };*/
        Console.WriteLine("Mot de passe : "  + DecrypterMotDePasse("TQBRAEEAMQBBAEQASQBBAE4AdwBCADAAQQBHADAAQQA="));
        Console.WriteLine(user.Role + " " + user.Pseudo);
        if (user == null)
            return null;
        return user;
    }

    public UserLoginResponseDTO GetUserLoginById(int idUserLogin)
    {
        var user = _repository.GetById(idUserLogin);
        UserLoginResponseDTO dto = new UserLoginResponseDTO{
            Pseudo = user.Pseudo,
            Role = user.Role
        };
        if (user == null)
            return null;
        return dto;
    }

    public void AddUserLogin(UserLoginRequestDTO userLogin)
    {
        Console.WriteLine("Service User Login");
        UserLogin user = new UserLogin{
            Pseudo = userLogin.Pseudo,
            Role = userLogin.Role,
            MotDePasse = CrypterMotDePasse(userLogin.MotDePasse)
        };
        _repository.Add(user);
    }

    public void UpdateUserLogin(UserLoginRequestDTO userLogin)
    {
        UserLogin user = new UserLogin{
            Pseudo = userLogin.Pseudo,
            Role = userLogin.Role,
            MotDePasse = CrypterMotDePasse(userLogin.MotDePasse)
        };
        _repository.Update(user);
    }

    public void RemoveUserLogin(int idUserLogin)
    {
        _repository.Remove(idUserLogin);
    }
}