using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PortofolioApi.Domain.DTOs;
using Microsoft.JSInterop;
using PortofolioApi.Services;

namespace PortofolioApi.Components.Pages;

public class AddContactBase : ComponentBase
{
    protected ContactDTO contact = new ContactDTO{ Utilisateur = new UtilisateurDTO() };
    protected List<ContactDTO> contacts = new List<ContactDTO>();
    protected string? message;
    protected bool editing = false;

    [Inject]
    protected HttpClient Http { get; set; }
    [Inject]
    protected IJSRuntime JS { get; set; }
    [Inject]
    protected UserState userState{ get; set; }
    [Inject] 
    protected NavigationManager Nav{get;set;}

    protected override async Task OnInitializedAsync()
    {
        if(userState.Role==null)
            Nav.NavigateTo("/login");
        else if(userState.Role.ToUpper()=="ADMIN")
            Nav.NavigateTo("/createuserlogin");
        try { contacts = await Http.GetFromJsonAsync<List<ContactDTO>>("api/contact"); } catch { }
    }

    protected async Task Save()
    {
        try
        {
            if (!editing)
            {
                var resp = await Http.PostAsJsonAsync("api/contact", contact);
                if (resp.IsSuccessStatusCode) message = "Contact ajouté";
                else message = "Erreur ajout";
            }
            else
            {
                var id = contact.IdContact ?? 0;
                var resp = await Http.PutAsJsonAsync($"api/contact/{id}", contact);
                if (resp.IsSuccessStatusCode) message = "Contact mis à jour";
                else message = "Erreur mise à jour";
            }
            contact = new ContactDTO{ Utilisateur = new UtilisateurDTO() };
            editing = false;
            contacts = await Http.GetFromJsonAsync<List<ContactDTO>>("api/contact");
        }
        catch (Exception ex)
        {
            message = ex.Message;
        }
    }

    protected void Edit(ContactDTO c)
    {
        contact = new ContactDTO
        {
            IdContact = c.IdContact,
            TypeContact = c.TypeContact,
            AdresseContact = c.AdresseContact,
            Utilisateur = c.Utilisateur ?? new UtilisateurDTO(),
            IdUser = c.IdUser
        };
        Console.WriteLine($"Edit contact: {contact.IdContact}, {contact.TypeContact}, {contact.AdresseContact}");
        editing = true;
    }

    protected async Task Delete(int id)
    {
        if (id <= 0) return;
        var ok = await JS.InvokeAsync<bool>("confirm", "Supprimer ce contact ?");
        if (!ok) return;
        var resp = await Http.DeleteAsync($"api/contact/{id}");
        if (resp.IsSuccessStatusCode)
        {
            contacts = await Http.GetFromJsonAsync<List<ContactDTO>>("api/contact");
        }
    }

    protected void CancelEdit()
    {
        editing = false;
        contact = new ContactDTO{ Utilisateur = new UtilisateurDTO() };
    }
}
