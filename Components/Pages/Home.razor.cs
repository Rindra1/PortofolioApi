using Microsoft.AspNetCore.Components;
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
    public LocalizationService? Localizer { get; set; }

    private readonly string url = $"api/portfolio";
    private bool isReady = false;

    [Inject]
    private IMemoryCache Cache { get; set; } = default!;

    private bool isLoading = false;

    private bool jsReady;

    protected override async Task OnInitializedAsync()
    {
        if (isReady) return;
        Localizer?.OnChange += OnLangChanged;
        const string cacheKey = "PortfolioData";
        if (Cache.TryGetValue(cacheKey, out UtilisateurDTO? cachedData) && cachedData != null)
        {
            try { await Localizer.InitializeAsync(); } catch { }
            portfolio = cachedData;
            originalPortfolio = cachedData;
            isReady = true;
            StateHasChanged();
            return;
        }
        portfolio = await Http!.GetFromJsonAsync<UtilisateurDTO>(url)
                    ?? new UtilisateurDTO();

        originalPortfolio = portfolio;
        Cache.Set("PortfolioData", portfolio, TimeSpan.FromMinutes(10));
        isReady = true;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender || jsReady)
            return;

        jsReady = true;

        await Localizer!.InitializeAsync();

        await InvokeAsync(StateHasChanged);
    }

    private void OnLangChanged()
    {
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