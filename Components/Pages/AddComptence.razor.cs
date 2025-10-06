using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PortofolioApi.Domain.DTOs;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Hosting;
using PortofolioApi.Services;

namespace PortofolioApi.Components.Pages;

public class AddComptenceBase : ComponentBase
{
    protected ExperienceDTO newExperience = new ExperienceDTO();
    protected List<ExperienceDTO> listeExperience = new List<ExperienceDTO>();
    protected string? message;
    [Inject]
    protected HttpClient Http { get; set; }
    [Inject]
    protected UserState userState { get; set; }
    [Inject]
    protected NavigationManager Nav { get; set; }
    [Inject]
    protected IJSRuntime JS { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (userState.Role == null)
            Nav.NavigateTo("/login");
        else if (userState.Role.ToUpper() == "ADMIN")
            Nav.NavigateTo("/createuserlogin");
        listeExperience = await Http.GetFromJsonAsync<List<ExperienceDTO>>("api/experience");
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JS.InvokeVoidAsync("siteInterop.initAll");
        }
    }
    protected async Task Enregistrer()
    {
        try
        {
            var response = await Http.PostAsJsonAsync("api/experience", newExperience);
            if (response.IsSuccessStatusCode)
            {
                message = "Expérience ajoutée avec succès !";
                newExperience = new ExperienceDTO(); // Réinitialiser le formulaire
                listeExperience = await Http.GetFromJsonAsync<List<ExperienceDTO>>("api/experience");
            }
            else
            {
                message = "Erreur lors de l'ajout de l'expérience.";
            }
        }
        catch (Exception ex)
        {
            message = $"Erreur: {ex.Message}";
        }
    }
}
