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

    protected List<IBrowserFile?> ListeFichier = new List<IBrowserFile>();
    protected IBrowserFile? Fichier;
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
            await JS.InvokeVoidAsync("siteInterop.initAll");
        }
    }
    // Méthode pour ajouter un lien à la liste
    protected void AjouterLien()
    {
        /*if (FichierLien != null)
        {
            ListeFichier.Add(FichierLien); // Ajouter le fichier à la liste
            FichierLien = null;             // réinitialiser l'input seulement après ajout
        }*/
    }

    // Sélection fichier projet
    protected void OnFileSelected(InputFileChangeEventArgs e)
    {
        Fichier = e.File;
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

        // 2️⃣ Création du projet côté serveur
        var response = await Http.PostAsJsonAsync("api/projet", newProjet);

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
        message = "Projet créé avec succès!";
        newProjet = new ProjetDTO();
        Fichier = null;
        ListeFichier.Clear();
        fileInputKey = Guid.NewGuid(); // reset input file
    }
    catch (Exception ex)
    {
        message = $"Une erreur s'est produite: {ex.Message}\n{ex.StackTrace}";
        Console.WriteLine(message);
    }
    }

}
