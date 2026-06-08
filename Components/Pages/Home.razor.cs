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

    private bool _initialized;
    protected override void OnInitialized()
    {
        Localizer?.OnChange += OnLangChanged;
        StateHasChanged();
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
                        //await TranslatePortfolioAsync(portfolio, Localizer.CurrentLanguage);
                    }
                    else
                    {
                        try
                        {
                            var url = $"api/portfolio";
                            var original = await Http.GetFromJsonAsync<UtilisateurDTO>(url);
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


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !_initialized)
        {
            _initialized = true;
            await JS.InvokeVoidAsync("siteInterop.initAll");
            try
            {
                string url = $"api/portfolio";
                try { await Localizer.InitializeAsync(); } catch { }
                portfolio = await Http.GetFromJsonAsync<UtilisateurDTO>(url) ?? new UtilisateurDTO();
                StateHasChanged();
                if (Localizer?.CurrentLanguage != "fr" && portfolio != null)
                {
                    //try { await TranslatePortfolioAsync(portfolio, Localizer.CurrentLanguage); StateHasChanged(); } catch { }
                }
                await JS.InvokeVoidAsync("siteInterop.initAll");
            }
            catch (Exception ex)
            {
                message = $"Erreur: {ex.Message} {ex.StackTrace}";
            }
            // Hide the page-specific loader when render/data load is complete
            try { await JS.InvokeVoidAsync("siteInterop.hideById", "loader-home"); } catch { }
        }
    }

    

    /*private async Task TranslatePortfolioAsync(UtilisateurDTO p, string target)
    {
        if (p == null) return;
        try
        {
            p.APropos = Localizer.T("APropos");
            p.resume = Localizer.T("Resumer");
            p.Titre = Localizer.T("Title");
        }catch (Exception ex){}

        // Translate projects
        if (p.Projets != null)
        {
            foreach (var proj in p.Projets)
            {
                try
                {
                    var idPart = proj.IdProjet?.ToString() ?? "0";
                    var titleKey = $"Projet.{idPart}.Titre";
                    var summaryKey = $"Projet.{idPart}.Resumer";
                    var detailKey = $"Projet.{idPart}.Detail";
                    var fonctionnaliteKey = $"Projet.{idPart}.Fonctionnalite";

var tTitle = Localizer.T(titleKey);
                    if (tTitle != titleKey) proj.TitreProjet = tTitle;

                    var tSummary = Localizer.T(summaryKey);
                    if (tSummary != summaryKey) proj.ResumerProjet = tSummary;

                    var tDetail = Localizer.T(detailKey);
                    if (tDetail != detailKey) proj.DetailProjet = tDetail;

                    var tFonctionnalite = Localizer.T(fonctionnaliteKey);
                    if (tFonctionnalite != fonctionnaliteKey) proj.Fonctionnalite = tFonctionnalite;

                }catch { }
            }
        }
        // Translate experiences
        if (p.Experiences != null)
        {
            foreach (var exp in p.Experiences)
            {
                try
                {
                    var idPart = exp.IdExperience.ToString();
                    var titreKey = $"Experience.{idPart}.Titre";
                    var detailKey = $"Experience.{idPart}.Detail";

                    var tt = Localizer.T(titreKey);
                    if (tt != titreKey) exp.TitreExperience = tt;

                    var td = Localizer.T(detailKey);
                    if (td != detailKey) exp.DetailExperience = td;
                }catch { }
            }
        }
        await Task.CompletedTask;
    }*/

    
}