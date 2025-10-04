using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PortofolioApi.Domain.Entities;
using PortofolioApi.Domain.DTOs;
using Microsoft.JSInterop;
using PortofolioApi.Services;



public partial class UtilisateurBase : ComponentBase
{
    [Inject]
    protected HttpClient Http {get;set;}
    [Inject]
    protected IJSRuntime JS {get;set;}
    [Inject] 
    protected NavigationManager Nav{get;set;}
    [Inject]
    protected UserState userState{ get; set; }
    protected IHttpClientFactory ClientFactory { get; set; } = default!;
    
    protected UserLoginResponseDTO newUser = new UserLoginResponseDTO();
    protected ContactDTO newContact = new ContactDTO();
    protected UtilisateurDTO newUtilisateur = new UtilisateurDTO();
    protected List<ContactDTO> contacts = new List<ContactDTO>();
    protected IBrowserFile? Fichier;
    protected string? chemin;
    protected string? message;
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if(userState.Role==null)
                Nav.NavigateTo("/login");
            else if(userState.Role.ToUpper()=="ADMIN")
                Nav.NavigateTo("/createuserlogin");
        }
    }

    // Méthode appelée au clic
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

            // Réinitialiser le formulaire
            newContact = new ContactDTO();
        }
    }


    protected override async Task OnInitializedAsync()
    {
        try
        {
            if(userState.Role==null)
                Nav.NavigateTo("/login");
            else if(userState.Role.ToUpper()=="ADMIN")
                Nav.NavigateTo("/createuserlogin");
    
            int id = 1;
            string url = $"https://localhost:7047/api/utilisateur/";
        
            HttpResponseMessage rep =  await Http.GetAsync(url);
            string json = await rep.Content.ReadAsStringAsync();
            //newUser = JsonSerializer.Deserialize<UserLogin>(json, new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
            //newUser = await Http.GetFromJsonAsync<UserLoginResponseDTO>(url);
        }catch(Exception ex)
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
                var uploadFolder = Path.Combine(Environment.CurrentDirectory,"wwwroot/images");
                if (!Directory.Exists(uploadFolder)){
                    Directory.CreateDirectory(uploadFolder);
                }
                var fileName= $"{Guid.NewGuid()}_{Fichier.Name}";
                var filePath = Path.Combine(uploadFolder, fileName);
                using var stream = File.Create(filePath);
                await Fichier.OpenReadStream().CopyToAsync(stream);
                newUtilisateur.UserImage = fileName;
                //newUtilisateur.IdUserLogin = 1;
                //newUtilisateur.UserLogin = newUser;
            }
            newUtilisateur.ContactDTOs = contacts; 

            //var client = ClientFactory.CreateClient("API");
            var user = await Http.PostAsJsonAsync("api/utilisateur", newUtilisateur);
            if (user.IsSuccessStatusCode)
            {
                /*int utilisateurId = await user.Content.ReadFromJsonAsync<int>();
                foreach(var contact in contacts)
                {
                    contact.Utilisateur = newUtilisateur;
                    var userlogin = await Http.PostAsJsonAsync("api/contact", contact);
                }*/
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
        catch (Exception ex)
        {
            message = $"Une erreur s'est produite: {ex.Message}";
        }
    }
}
