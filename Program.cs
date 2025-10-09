using PortofolioApi.Components;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PortofolioApi.Domain.Entities;
using PortofolioApi.Domain.DTOs;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using PortofolioApi.Application.Services;
using PortofolioApi.Infrastructure.Data;
using PortofolioApi.Configuration;
using PortofolioApi.Domain.Interfaces;
using PortofolioApi.Services;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;

using System.Net;
using Microsoft.AspNetCore.Mvc;




// Chemin vers la base, compatible local et Render
var appDataFolder = Path.Combine(AppContext.BaseDirectory, "var", "data");
Directory.CreateDirectory(appDataFolder); // Crée le dossier si inexistant

var dbPath = Path.Combine(appDataFolder, "appdata.db");
Console.WriteLine($"Chemin de la base de données: {dbPath}");
// Supprime le fichier existant s'il existe
/*if (File.Exists(dbPath))
{
    File.Delete(dbPath);
}*/
// Téléchargement si la base n'existe pas
if (!File.Exists(dbPath))
{
    Console.WriteLine("Téléchargement de la base de données depuis GitHub...");
    using var client = new WebClient();

    try
    {
        client.DownloadFile(
            "https://github.com/Rindra1/PortofolioApi/raw/refs/heads/main/appdata.db",
            dbPath
        );
        Console.WriteLine("Base téléchargée avec succès !");
        Console.WriteLine(new FileInfo(dbPath).Length);

    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erreur lors du téléchargement de la base : {ex.Message}");
    }
}






var builder = WebApplication.CreateBuilder(args);

//Utilisation Open-API
//var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

//builder.Services.AddSingleton(new OpenAI.OpenAIClient(apiKey));

//Test en local Open-api
/*builder.Services.AddSingleton(new OpenAI.GPT3.OpenAIService(new OpenAI.GPT3.Models.OpenAiOptions()
{
    ApiKey = builder.Configuration["OpenAI:ApiKey"]
}));*/





// Ajoute cette ligne juste après la création du builder
builder.Configuration.AddEnvironmentVariables();
builder.Environment.EnvironmentName = Environments.Development;

// Liaison du fichier appsettings.json ou des variables Render
builder.Services.Configure<SendGridSettings>(
    builder.Configuration.GetSection("SendGridSettings")
);

//Injection du service SendGrid
builder.Services.AddScoped<SendGridEmailService>();


builder.Services.Configure<SendGridSettings>(builder.Configuration.GetSection("SendGrid"));
builder.Services.AddTransient<SendGridEmailService>();


// Injection du service mail
//builder.Services.AddSingleton<MailController>();

// Récupère le port fourni par Render
//var port = Environment.GetEnvironmentVariable("PORT") ?? "5000"; // 5000 si local
//sbuilder.WebHost.UseUrls($"http://*:{port}");

//builder.Services.AddSingleton<UserState>();
//builder.Services.AddSingleton<PortofolioApi.Services.TokenServices>();


builder.Services.AddScoped<UserState>();
builder.Services.AddScoped<PortofolioApi.Services.TokenServices>();

// Bind JwtSettings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Add TokenService
builder.Services.AddScoped<ITokenService, TokenService>();

// Add HttpContext accessor & CurrentUserService (optionnel)
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
var key = Encoding.UTF8.GetBytes(jwtSettings.Secret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,

        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),

        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(2)
    };
});
builder.Services.AddAuthorization();
builder.Services.AddControllers();
// ... other services



// Configurer Sqlite
/*builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")));
*/
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite($"Data Source={dbPath}");
});



// Ajouter les services applicatifs
builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
builder.Services.AddScoped<UserLoginService>();
builder.Services.AddScoped<UtilisateurService>();
builder.Services.AddScoped<LienService>();
builder.Services.AddScoped<ProjetService>();
builder.Services.AddScoped<ContactService>();
builder.Services.AddScoped<ExperienceService>();
/*Envoi Email*/
builder.Services.AddScoped<SendGridEmailService>();
builder.Services.AddScoped<SendGridSettings>();
/**********************/
builder.Services.AddScoped<PortfolioService>();
/*Storage*/
builder.Services.AddScoped<ProtectedLocalStorage>();
//ChatBot
builder.Services.AddScoped<ChatService>();


// Repository
builder.Services.AddScoped<IRepository<UserLogin>, UserLoginRepository>();
builder.Services.AddScoped<IRepository<Utilisateur>, UtilisateurRepository>();
builder.Services.AddScoped<IRepositoryPortfolio<UtilisateurDTO>, PortfolioRepository>();
builder.Services.AddScoped<IRepository<Projet>, ProjetRepository>();
builder.Services.AddScoped<IRepository<Lien>, LienRepository>();
builder.Services.AddScoped<IRepository<Contact>, ContactRepository>();
builder.Services.AddScoped<IRepository<Experience>, ExperienceRepository>();
builder.Services.AddScoped<IRepositoryPortfolio<UtilisateurDTO>, PortfolioRepository>();
builder.Services.AddScoped<IRepository<Competence>, CompetenceRepository>();



builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true; // optionnel
    });


// Ajouter les services API et HttpClient
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();



builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri("https://localhost:7047/"); //En local
    //client.BaseAddress = new Uri("https://rindra-dotnet-developer.onrender.com"); //Sur Render
    client.Timeout = TimeSpan.FromSeconds(60); // éviter le TaskCanceled
});

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));

// Ajouter les services Blazor Server et Razor Components
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


// Ajouter les services Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
});


var app = builder.Build();

// Route de test pour vérifier si le serveur est actif
app.MapGet("/health", () => Results.Ok("OK"));


// Pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();    // 🔹 Toujours avant Authorization
app.UseAuthorization();
app.UseAntiforgery();




// Afficher tous les endpoints dans la console
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();

    // Ajoute un endpoint qui affiche toutes les routes disponibles
    endpoints.MapGet("/routes", async context =>
    {
        var dataSource = context.RequestServices.GetRequiredService<EndpointDataSource>();
        var routes = dataSource.Endpoints
            .OfType<RouteEndpoint>()
            .Select(e => e.RoutePattern.RawText);

        await context.Response.WriteAsync("Endpoints:\n" + string.Join("\n", routes));
    });
});

//app.UseCors("AllowAll");  // 🔹 Mettre ça avant app.UseAuthorization()



// Routing API et Razor
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Portofolio API V1");
    });
}

/*app.MapBlazorHub(options =>
{
    // Forcer LongPolling si WebSockets ne sont pas supportés
    options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling;
});*/


app.Run();
