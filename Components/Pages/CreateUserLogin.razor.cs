using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PortofolioApi.Domain.Entities;
using PortofolioApi.Domain.DTOs;
using PortofolioApi.Services;


public partial class CreateUserLoginBase : ComponentBase
{
    [Inject]
    protected HttpClient Http {get;set;}
    [Inject]
    protected UserState userState{ get; set; }
    [Inject] 
    protected NavigationManager Nav{get;set;}
    protected UserLoginRequestDTO newUserLogin = new UserLoginRequestDTO{Role = "Administrateur"};
    protected List<UserLoginResponseDTO> users = new List<UserLoginResponseDTO>();
    protected List<string> roles = new(){"Administrateur","Utilisateur"};
   protected string message = string.Empty;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if(userState.Role==null)
                Nav.NavigateTo("/login");
        }
    }

    protected override async Task OnInitializedAsync()
    {
        string url = $"api/userlogin";
        users = await Http.GetFromJsonAsync<List<UserLoginResponseDTO>>(url) ?? null;

    }


    protected async Task CreerUserLogin()
    {
        try
        {
            //var client = ClientFactory.CreateClient("API");
            var response = await Http.PostAsJsonAsync("https://localhost:7047/api/userlogin", newUserLogin);
            if (response.IsSuccessStatusCode)
            {
                users.Add(new UserLoginResponseDTO{
                    Pseudo = newUserLogin.Pseudo,
                    Role = newUserLogin.Role
                });
                message = "Utilisateur créé avec succès!";
                newUserLogin = new UserLoginRequestDTO(); // Réinitialiser le formulaire
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
