using Microsoft.AspNetCore.Components;
using PortofolioApi.Domain.DTOs;
using PortofolioApi.Services;

namespace PortofolioApi.Components.Shared
{
    public partial class Projects
    {
        [Parameter]
        public UtilisateurDTO portfolio { get; set; }
        [Parameter]
        public LocalizationService Localizer { get; set; }
        private int projet = 0;
        private int projetdetail = 0;
    }
}
