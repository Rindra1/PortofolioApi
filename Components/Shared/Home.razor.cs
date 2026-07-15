using Microsoft.AspNetCore.Components;
using PortofolioApi.Domain.DTOs;
using PortofolioApi.Services;
using Microsoft.Extensions.Caching.Memory;

namespace PortofolioApi.Components.Shared;
public partial class Home
{
    [Parameter]
    public UtilisateurDTO? portfolio { get; set; }

    [Parameter]
    public LocalizationService? Localizer { get; set; }

    [Inject]
    private IMemoryCache Cache { get; set; } = default!;

    private bool IsLoading = true;

    public string resume = "";

    protected override void OnParametersSet()
    {
        resume = Localizer?.T("Resumer") ?? resume;
    }

    protected override async Task OnInitializedAsync()
    {
        // Si les données sont déjà passées par le parent
        if (portfolio != null)
        {
            IsLoading = false;
            return;
        }

        // Vérifier le cache
        const string cacheKey = "PortfolioData";
        if (Cache.TryGetValue(cacheKey, out UtilisateurDTO? cachedData) && cachedData != null)
        {
            portfolio = cachedData;
            IsLoading = false;
            return;
        }

        // Charger depuis l'API en arrière-plan (ne bloque pas le rendu)
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            const string cacheKey = "PortfolioData";
            
            var response = await Http.GetFromJsonAsync<UtilisateurDTO>("/api/portfolio");
            
            if (response != null)
            {
                portfolio = response;
                Cache.Set(cacheKey, response, TimeSpan.FromMinutes(5));
            }
            else
            {
                portfolio = GetFallbackData();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur: {ex.Message}");
            portfolio = GetFallbackData();
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    private UtilisateurDTO GetFallbackData()
    {
        return new UtilisateurDTO
        {
            Nom = "Razafimandanona",
            Prenom = "Rindra Niaina",
            UserImage = "profile.jpg",
            StackMaitrise = "C#, .NET, Blazor, ASP.NET Core, SQL Server, Entity Framework, Docker"
        };
    }
}