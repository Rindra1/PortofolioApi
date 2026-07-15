using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using PortofolioApi.Domain.DTOs;
using PortofolioApi.Services;

namespace PortofolioApi.Components.Pages;
public partial class Home
{
    public UtilisateurDTO portfolio = new UtilisateurDTO();
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

    protected override async Task OnInitializedAsync()
    {
        Localizer?.OnChange += OnLangChanged;
        
        try
        {
            try { await Localizer.InitializeAsync(); } catch { }
            portfolio = await Http.GetFromJsonAsync<UtilisateurDTO>(url) ?? new UtilisateurDTO();
            if (Localizer?.CurrentLanguage != "fr" && portfolio != null)
            {
                //try { await TranslatePortfolioAsync(portfolio, Localizer.CurrentLanguage); StateHasChanged(); } catch { }
            }
        }
        catch (Exception ex)
        {
            message = $"Erreur: {ex.Message}";
            Console.WriteLine($"Erreur chargement portfolio: {ex.Message}");
        }
        // Hide the page-specific loader when render/data load is complete
        //try { await JS.InvokeVoidAsync("siteInterop.hideById", "loader-home"); } catch { }
    }

    private void OnLangChanged()
    {
        InvokeAsync(async () =>
        {
            try
            {
                if (portfolio != null)
                {
                    if (Localizer?.CurrentLanguage != "fr")
                    {
                    }
                    else
                    {
                        try
                        {
                            var original = await Http?.GetFromJsonAsync<UtilisateurDTO>(url);
                            if (original != null) portfolio = original;
                        }
                        catch { }
                    }
                }
            }
            catch { }
            StateHasChanged();
        });
    }

    public void Dispose() => Localizer?.OnChange -= OnLangChanged;
    
}