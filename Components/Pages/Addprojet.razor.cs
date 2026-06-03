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


public class AddprojetBase : ComponentBase
{
    [Inject]
    protected HttpClient Http {get;set;}
    [Inject]
    protected UserState userState{ get; set; }
    [Inject] 
    protected NavigationManager Nav{get;set;}
    protected ProjetDTO newProjet = new ProjetDTO();
    protected LienDTO newLien = new LienDTO();
    protected List<LienDTO> listeLien = new List<LienDTO>();
    protected List<UserLoginResponseDTO> newUser = new List<UserLoginResponseDTO>();
    protected List<ProjetDTO> projets = new List<ProjetDTO>();
    protected bool editing = false;

    protected List<IBrowserFile?> ListeFichier = new List<IBrowserFile>();
    protected IBrowserFile? Fichier;
    protected IBrowserFile? Fichier1;
    protected IBrowserFile? Fichier2;
    protected IBrowserFile? FichierLien;
    protected string? chemin;
    protected string? message;  
    protected Guid fileInputKey = Guid.NewGuid(); // Pour réinitialiser l'input file   
    [Inject] 
    protected IJSRuntime JS { get; set; }
    [Inject] 
    protected IWebHostEnvironment _env{get;set;}

    protected override async Task OnInitializedAsync()
    {
        if(userState.Role==null)
            Nav.NavigateTo("/login");
        else if(userState.Role.ToUpper()=="ADMIN")
            Nav.NavigateTo("/createuserlogin");
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            //await JS.InvokeVoidAsync("siteInterop.initAll");
            // charger projets existants
            try { projets = await Http.GetFromJsonAsync<List<ProjetDTO>>("api/projet"); } catch { }
            StateHasChanged();
        }
    }

    // Méthode pour initialiser et charger les projets existants (déjà déclarée plus haut)
    // Méthode pour ajouter un lien à la liste
    protected void AjouterLien()
    {
        /*if (FichierLien != null)
        {
            ListeFichier.Add(FichierLien); // Ajouter le fichier à la liste
            FichierLien = null;             // réinitialiser l'input seulement après ajout
        }*/
    }

    protected void EditProjet(ProjetDTO p)
    {
        newProjet = new ProjetDTO
        {
            IdProjet = p.IdProjet,
            TitreProjet = p.TitreProjet,
            ResumerProjet = p.ResumerProjet,
            DetailProjet = p.DetailProjet,
            ImageProjet = p.ImageProjet,
            ImageProjet1 = p.ImageProjet1,
            ImageProjet2 = p.ImageProjet2,
            Stack = p.Stack,
            Lien = p.Lien,
            Fonctionnalite = p.Fonctionnalite
        };
        editing = true;
    }

    protected async Task SupprimerProjet(int id)
    {
        if (id <= 0) return;
        var ok = await JS.InvokeAsync<bool>("confirm", "Supprimer ce projet ?");
        if (!ok) return;
        var response = await Http.DeleteAsync($"api/projet/{id}");
        if (response.IsSuccessStatusCode)
        {
            try { projets = await Http.GetFromJsonAsync<List<ProjetDTO>>("api/projet"); } catch { }
        }
    }
    // Sélection fichier projet
    protected void OnFileSelected(InputFileChangeEventArgs e)
    {
        Fichier = e.File;
    }

    // Sélection image secondaire 1
    protected void OnFileSelectedImage1(InputFileChangeEventArgs e)
    {
        Fichier1 = e.File;
    }

    // Sélection image secondaire 2
    protected void OnFileSelectedImage2(InputFileChangeEventArgs e)
    {
        Fichier2 = e.File;
    }

    // Sélection fichier lien
    protected void OnFileSelectedLien(InputFileChangeEventArgs e)
    {
        foreach (var file in e.GetMultipleFiles())
        {
            ListeFichier.Add(file);
        }

        // Réinitialiser InputFile pour pouvoir resélectionner les mêmes fichiers
        fileInputKey = Guid.NewGuid();
    }


    // Enregistrement projet + liens
    protected async Task Enregistrer()
{
    Console.WriteLine("Enregistrement projet");
    Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(newProjet));

    try
    {
        // 1️⃣ Upload image projet
        if (Fichier != null)
        {
            var uploadFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "images");
            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(Fichier.Name)}";
            var filePath = Path.Combine(uploadFolder, fileName);

            using var stream = File.Create(filePath);
            await Fichier.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024).CopyToAsync(stream);

            newProjet.ImageProjet = fileName;
            Console.WriteLine($"Image projet uploadée: {fileName}");
        }

        // Upload image secondaire 1
        if (Fichier1 != null)
        {
            var uploadFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "images");
            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(Fichier1.Name)}";
            var filePath = Path.Combine(uploadFolder, fileName);

            using var stream1 = File.Create(filePath);
            await Fichier1.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024).CopyToAsync(stream1);
            newProjet.ImageProjet1 = fileName;
            Console.WriteLine($"ImageProjet1 uploadée: {fileName}");
        }

        // Upload image secondaire 2
        if (Fichier2 != null)
        {
            var uploadFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "images");
            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(Fichier2.Name)}";
            var filePath = Path.Combine(uploadFolder, fileName);

            using var stream2 = File.Create(filePath);
            await Fichier2.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024).CopyToAsync(stream2);
            newProjet.ImageProjet2 = fileName;
            Console.WriteLine($"ImageProjet2 uploadée: {fileName}");
        }

        // 2️⃣ Création / mise à jour du projet côté serveur
        HttpResponseMessage response;
        if (!editing)
        {
            response = await Http.PostAsJsonAsync("api/projet", newProjet);
        }
        else
        {
            var idToUpdate = newProjet.IdProjet ?? 0;
            // build object matching Projet entity expected by API
            var projetObj = new
            {
                IdProjet = idToUpdate,
                ResumerProjet = newProjet.ResumerProjet,
                TitreProjet = newProjet.TitreProjet,
                DetailProjet = newProjet.DetailProjet,
                ImageProjet = newProjet.ImageProjet,
                ImageProjet1 = newProjet.ImageProjet1,
                ImageProjet2 = newProjet.ImageProjet2,
                Stack = newProjet.Stack,
                Lien = newProjet.Lien,
                Fonctionnalite = newProjet.Fonctionnalite
            };
            response = await Http.PutAsJsonAsync($"api/projet/{idToUpdate}", projetObj);
        }

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            message = $"Erreur lors de la création du projet: {response.StatusCode} - {errorContent}";
            Console.WriteLine(message);
            return;
        }

        int idProjet;
        try
        {
            idProjet = await response.Content.ReadFromJsonAsync<int>();
        }
        catch (Exception)
        {
            // Si le backend retourne { "id": 1 } au lieu de juste 1
            var json = await response.Content.ReadAsStringAsync();
            idProjet = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string,int>>(json)?["id"] ?? 0;
        }

        Console.WriteLine($"Projet créé avec ID: {idProjet}");

        // 3️⃣ Upload fichiers liens
        foreach (var fichier in ListeFichier.Where(f => f != null))
        {
            var uploadFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "images");
            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(fichier!.Name)}";
            var filePath = Path.Combine(uploadFolder, fileName);

            using var stream = File.Create(filePath);
            await fichier!.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024).CopyToAsync(stream);

            var lien = new LienDTO
            {
                CheminLien = fileName,
                IdProjet = idProjet
            };
            await Http.PostAsJsonAsync("api/lien", lien);
            Console.WriteLine($"Lien uploadé: {fileName}");
        }

        // 4️⃣ Réinitialisation
        message = editing ? "Projet mis à jour avec succès!" : "Projet créé avec succès!";
        newProjet = new ProjetDTO();
        editing = false;
        Fichier = null;
        ListeFichier.Clear();
        fileInputKey = Guid.NewGuid(); // reset input file
        try { projets = await Http.GetFromJsonAsync<List<ProjetDTO>>("api/projet"); } catch { }
    }
    catch (Exception ex)
    {
        message = $"Une erreur s'est produite: {ex.Message}\n{ex.StackTrace}";
        Console.WriteLine(message);
    }
    }

}
