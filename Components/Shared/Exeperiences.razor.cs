using Microsoft.AspNetCore.Components;
using PortofolioApi.Domain.DTOs;
using PortofolioApi.Services;

namespace PortofolioApi.Components.Shared
{
    public partial class Exeperiences
    {
        [Parameter]
        public UtilisateurDTO portfolio { get; set; }
        [Parameter]
        public LocalizationService Localizer { get; set; }
    }
}
