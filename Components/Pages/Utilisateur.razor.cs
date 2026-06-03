using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PortofolioApi.Domain.Entities;
using PortofolioApi.Domain.DTOs;
using Microsoft.JSInterop;
using PortofolioApi.Services;
using System.Linq;
using System.IO;
using System;
using System.Collections.Generic;

public partial class UtilisateurBase : ComponentBase
{
    [Inject]
    protected HttpClient Http { get; set; } = default!;
    [Inject]
    protected IJSRuntime JS { get; set; } = default!;
    [Inject]
    protected NavigationManager Nav { get; set; } = default!;
    [Inject]
    protected UserState userState { get; set; } = default!;

    protected UserLoginResponseDTO newUser = new UserLoginResponseDTO();
    protected ContactDTO newContact = new ContactDTO();
    protected UtilisateurDTO newUtilisateur = new UtilisateurDTO();
    protected List<ContactDTO> contacts = new List<ContactDTO>();
    protected IBrowserFile? Fichier;
    protected string? chemin;
    protected string? message;
    protected bool editing = false;
    protected int? currentUserId = null;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            /*if (userState.Role == null)
                Nav.NavigateTo("/login");
            else if (userState.Role.ToUpper() == "ADMIN")
                Nav.NavigateTo("/createuserlogin");*/
        }
    }

    // Méthode appelée au clic pour ajouter un contact localement
    protected void AjouterContact()
    {
        if (!string.IsNullOrWhiteSpace(newContact.TypeContact) &&
            !string.IsNullOrWhiteSpace(newContact.AdresseContact))
        {
            contacts.Add(new ContactDTO
            {
                TypeContact = newContact.TypeContact,
                AdresseContact = newContact.AdresseContact
            });

            newContact = new ContactDTO();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            if (userState.Role == null)
                Nav.NavigateTo("/login");
            else if (userState.Role.ToUpper() == "ADMIN")
                Nav.NavigateTo("/createuserlogin");

            // Try to load existing utilisateur (id=1 assumed).
            var id = 1;
            try
            {
                var exist = await Http.GetFromJsonAsync<PortofolioApi.Domain.Entities.Utilisateur>($"api/utilisateur/{id}");
                Console.WriteLine($"Utilisateur existant trouvé : {exist?.Nom} {exist?.Prenom} {exist?.IdUser}");
                if (exist != null)
                {
                    editing = true;
                    currentUserId = exist.IdUser;
                    newUtilisateur = new UtilisateurDTO
                    {
                        resume = exist.resume,
                        Nom = exist.Nom ?? string.Empty,
                        Prenom = exist.Prenom ?? string.Empty,
                        APropos = exist.APropos ?? string.Empty,
                        UserImage = exist.UserImage ?? string.Empty,
                        Titre = exist.Titre,
                        StackMaitrise = exist.StackMaitrise,
                        StackTechnique = exist.StackTechnique,
                        Contacts = exist.Contacts?.Select(c => new ContactDTO { TypeContact = c.TypeContact, AdresseContact = c.AdresseContact }).ToList() ?? new List<ContactDTO>()
                    };
                    contacts = newUtilisateur.Contacts ?? new List<ContactDTO>();
                }
            }
            catch
            {
                // ignore - no user yet
            }
        }
        catch (Exception ex)
        {
            message = $"Une erreur s'est produite: {ex.Message}";
        }
    }

    protected void OnFileSelected(InputFileChangeEventArgs e)
    {
        Fichier = e.File;
        chemin = null;
    }

    protected async Task Enregistrer()
    {
        try
        {
            if (Fichier != null)
            {
                var uploadFolder = Path.Combine(Environment.CurrentDirectory, "wwwroot/images");
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }
                var fileName = $"{Guid.NewGuid()}_{Fichier.Name}";
                var filePath = Path.Combine(uploadFolder, fileName);
                using var stream = File.Create(filePath);
                await Fichier.OpenReadStream().CopyToAsync(stream);
                newUtilisateur.UserImage = fileName;
            }

            newUtilisateur.Contacts = contacts;

            if (!editing)
            {
                var user = await Http.PostAsJsonAsync("api/utilisateur", newUtilisateur);
                if (user.IsSuccessStatusCode)
                {
                    message = "Utilisateur créé avec succès!";
                    newUtilisateur = new UtilisateurDTO(); // Réinitialiser le formulaire
                    newContact = new ContactDTO();
                    contacts = new List<ContactDTO>();
                }
                else
                {
                    message = "Erreur lors de la création de l'utilisateur.";
                }
            }
            else
            {
                var utilisateur = new PortofolioApi.Domain.Entities.Utilisateur
                {
                    IdUser = currentUserId ?? 0,
                    resume = newUtilisateur.resume,
                    Nom = newUtilisateur.Nom,
                    Prenom = newUtilisateur.Prenom,
                    APropos = newUtilisateur.APropos,
                    UserImage = newUtilisateur.UserImage,
                    Titre = newUtilisateur.Titre,
                    StackMaitrise = newUtilisateur.StackMaitrise,
                    StackTechnique = newUtilisateur.StackTechnique,
                    Contacts = contacts.Select(c => new PortofolioApi.Domain.Entities.Contact { TypeContact = c.TypeContact, AdresseContact = c.AdresseContact }).ToList()
                };

                var resp = await Http.PutAsJsonAsync($"api/utilisateur/{utilisateur.IdUser}", utilisateur);
                if (resp.IsSuccessStatusCode)
                {
                    message = "Utilisateur mis à jour avec succès!";
                }
                else
                {
                    message = "Erreur lors de la mise à jour de l'utilisateur.";
                }
            }
        }
        catch (Exception ex)
        {
            message = $"Une erreur s'est produite: {ex.Message}";
        }
    }

    protected void SupprimerContact(ContactDTO c)
    {
        contacts.Remove(c);
    }
}
