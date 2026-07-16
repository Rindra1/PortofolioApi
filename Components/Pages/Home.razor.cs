using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using PortofolioApi.Domain.DTOs;
using PortofolioApi.Services;

using Microsoft.Extensions.Caching.Memory;

namespace PortofolioApi.Components.Pages;
public partial class Home
{
    public UtilisateurDTO portfolio = new UtilisateurDTO();
    private UtilisateurDTO originalPortfolio = new UtilisateurDTO();
    protected string? message;

    [Inject]
    protected HttpClient? Http { get; set; }
    [Inject]
    protected IJSRuntime? JS { get; set; }
    [Inject] 
    protected NavigationManager? Nav{get;set;}
    [Inject]
    public LocalizationService? Localizer { get; set; }

    private readonly string url = $"api/portfolio";
    private bool isReady = false;

    [Inject]
    private IMemoryCache Cache { get; set; } = default!;

    /*protected override async Task OnInitializedAsync()
    {
        Localizer?.OnChange += OnLangChanged;
        Console.WriteLine("OnInitializedAsync");
        try
        {
                    // Vérifier le cache
            const string cacheKey = "PortfolioData";
            if (Cache.TryGetValue(cacheKey, out UtilisateurDTO? cachedData) && cachedData != null)
            {
                portfolio = cachedData;
                originalPortfolio = portfolio;
                isReady = true;
                return;
            }
            else
            {
                var localizationTask = Localizer.InitializeAsync();
                var portfolioTask = Http.GetFromJsonAsync<UtilisateurDTO>(url);

                //await Task.WhenAll(localizationTask, portfolioTask);

                portfolio = portfolioTask.Result ?? new UtilisateurDTO();
                originalPortfolio = portfolio;
                Cache.Set(cacheKey, portfolio, TimeSpan.FromMinutes(10));
                isReady = true;    
            }
        }
        catch (Exception ex)
        {
            message = $"Erreur: {ex.Message}";
            Console.WriteLine($"Erreur chargement portfolio: {ex.Message}");
        }
        
    }*/

    private bool initialized;
    private bool isLoading = false;

    protected override void OnInitialized()
    {
        Localizer?.OnChange -= OnLangChanged;
        Localizer?.OnChange += OnLangChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender || initialized)
            return;

        initialized = true;

        const string cacheKey = "PortfolioData";
        if (Cache.TryGetValue(cacheKey, out UtilisateurDTO? cachedData) && cachedData != null)
        {
            portfolio = cachedData;
            originalPortfolio = cachedData;
            isReady = true;
            StateHasChanged();
            return;
        }

        isLoading = true;
        StateHasChanged();

        await LoadPortfolioAsync();
    }

    private async Task LoadPortfolioAsync()
    {
        try
        {
            var localizationTask = Localizer?.InitializeAsync() ?? Task.CompletedTask;
            var portfolioTask = Http!.GetFromJsonAsync<UtilisateurDTO>(url);

            await Task.WhenAll(localizationTask, portfolioTask);

            portfolio = await portfolioTask ?? new UtilisateurDTO();
            originalPortfolio = portfolio;
            Cache.Set("PortfolioData", portfolio, TimeSpan.FromMinutes(10));
            isReady = true;
        }
        catch (Exception ex)
        {
            message = $"Erreur: {ex.Message}";
            Console.WriteLine($"Erreur chargement portfolio: {ex.Message}");
        }
        finally
        {
            isLoading = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    private void OnLangChanged()
    {
        Console.WriteLine("OnLangChanged");
        InvokeAsync(() =>
        {
            if (Localizer?.CurrentLanguage == "fr")
            {
                portfolio = originalPortfolio;
            }

            StateHasChanged();
        });
    }


    public void Dispose() => Localizer?.OnChange -= OnLangChanged;
    
}