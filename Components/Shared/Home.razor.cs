using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using PortofolioApi.Domain.DTOs;
using PortofolioApi.Services;

namespace PortofolioApi.Components.Shared;
public partial class Home
{
    [Parameter]
    public UtilisateurDTO? portfolio { get; set; }

    [Parameter]
    public LocalizationService? Localizer { get; set; }

    [Inject]
    protected IJSRuntime JS { get; set; }


    private async Task ScrollToSection(string sectionId)
    {
    await JS.InvokeVoidAsync("siteInterop.scrollToSection", sectionId);
    }
}