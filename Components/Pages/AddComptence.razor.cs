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
    protected bool editing = false;
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
            //await JS.InvokeVoidAsync("siteInterop.initAll");
        }
    }
    protected async Task Enregistrer()
    {
        Console.WriteLine("Nouvelle expérience à enregistrer :");
        try
        {
            if (!editing)
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
            else
            {
                // edition
                var id = newExperience.IdExperience;
                var response = await Http.PutAsJsonAsync($"api/experience/{id}", newExperience);
                if (response.IsSuccessStatusCode)
                {
                    message = "Expérience mise à jour !";
                    editing = false;
                    newExperience = new ExperienceDTO();
                    listeExperience = await Http.GetFromJsonAsync<List<ExperienceDTO>>("api/experience");
                }
                else
                {
                    message = "Erreur lors de la mise à jour.";
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur: {ex.Message}");
            message = $"Erreur: {ex.Message}";
        }
    }

    protected void EditExperience(ExperienceDTO exp)
    {
        newExperience = new ExperienceDTO
        {
            IdExperience = exp.IdExperience,
            TitreExperience = exp.TitreExperience,
            DetailExperience = exp.DetailExperience,
            DateDebut = exp.DateDebut,
            DateFin = exp.DateFin
        };
        editing = true;
    }

    protected async Task SupprimerExperience(int id)
    {
        if (id <= 0) return;
        var ok = await JS.InvokeAsync<bool>("confirm", "Supprimer cette expérience ?");
        if (!ok) return;
        var response = await Http.DeleteAsync($"api/experience/{id}");
        if (response.IsSuccessStatusCode)
        {
            listeExperience = await Http.GetFromJsonAsync<List<ExperienceDTO>>("api/experience");
        }
    }

    protected void AnnulerEdit()
    {
        editing = false;
        newExperience = new ExperienceDTO();
    }
}
