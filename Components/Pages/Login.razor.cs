using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PortofolioApi.Domain.Entities;
using PortofolioApi.Domain.DTOs;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using PortofolioApi.Services;


namespace PortofolioApi.Components.Pages;

public class LoginBase : ComponentBase
{
    [Inject]
    protected HttpClient Http{get;set;}
    [Inject] 
    protected NavigationManager Navigation{get;set;}
    [Inject]
    protected ProtectedLocalStorage Storage { get; set; }
    [Inject]
    protected UserState UserState { get; set; }
    [Inject]
    protected TokenServices TokenServices { get; set; }
    


    protected UserLoginRequestDTO userLogin = new UserLoginRequestDTO();
    protected string message = string.Empty;
    
    [Inject] 
    protected IJSRuntime JS { get; set; }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JS.InvokeVoidAsync("siteInterop.initAll");
        }
    }

private async Task RedirectAfterLogin()
{
    await JS.InvokeVoidAsync("eval", "window.location.href='/createuserlogin';");
}

    protected override async Task OnInitializedAsync()
{
    using var handler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
    };

    using var client = new HttpClient(handler)
    {
        Timeout = TimeSpan.FromSeconds(60)
    };

    //var response = await client.PostAsJsonAsync("https://localhost:7047/api/userlogin/authenticate",
        //new { Pseudo = "Administrateur", MotDePasse = "1527tm" , Role = "Role" });

    /*var result = await response.Content.ReadAsStringAsync();
    Console.WriteLine(result);
    Navigation.NavigateTo("/createuserlogin");*/
}


    
    protected async Task Connect()
    {
       try
        {
            // URL relative pour éviter double host / timeout
            userLogin.Role = "Role";
            Console.WriteLine(userLogin);
            var response = await Http.PostAsJsonAsync("/api/userlogin/authenticate", userLogin);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

                if (result == null)
                {
                    message = "Erreur lors de la récupération du token.";
                    return;
                }

                // Stocker le token côté client
                await JS.InvokeVoidAsync("localStorage.setItem", "token", result.Token);
                UserState.Role = result.Role; // 🔹 stocker le rôle dans UserState
                TokenServices.SetToken(result.Token); // 🔹 stocker le token et le rôle dans TokenService
                // Redirection selon le rôle
                StateHasChanged();
                if (TokenServices.GetRole().ToUpper() == "ADMINISTRATEUR")
                    Navigation.NavigateTo("/createuserlogin");
                else if (TokenServices.GetRole().ToUpper() == "ADMIN")
                    Navigation.NavigateTo("/createuserlogin");
                else
                    Navigation.NavigateTo("/addprojet");
            }
            else
            {
                Console.WriteLine("Pseudo ou mot de passe incorrect.");
                message = "Pseudo ou mot de passe incorrect.";
            }
        }
        catch (Exception ex)
        {
            message = $"Erreur: {ex.Message}";
            Console.WriteLine($"Erreur: {ex.Message} {ex.StackTrace}");
        }
    }
}
