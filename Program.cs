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
using Microsoft.OpenApi.Models;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;





var builder = WebApplication.CreateBuilder(args);

// Récupère le port fourni par Render
//var port = Environment.GetEnvironmentVariable("PORT") ?? "5000"; // 5000 si local
//sbuilder.WebHost.UseUrls($"http://*:{port}");

builder.Services.AddSingleton<UserState>();
builder.Services.AddSingleton<PortofolioApi.Services.TokenServices>();


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
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")));

// Ajouter les services applicatifs
builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
builder.Services.AddScoped<UserLoginService>();
builder.Services.AddScoped<UtilisateurService>();
builder.Services.AddScoped<LienService>();
builder.Services.AddScoped<ProjetService>();
builder.Services.AddScoped<ContactService>();
builder.Services.AddScoped<ExperienceService>();
builder.Services.AddScoped<PortfolioService>();

// Repository
builder.Services.AddScoped<IRepository<UserLogin>, UserLoginRepository>();
builder.Services.AddScoped<IRepository<Utilisateur>, UtilisateurRepository>();
builder.Services.AddScoped<IRepositoryPortfolio<UtilisateurDTO>, PortfolioRepository>();
builder.Services.AddScoped<IRepository<Projet>, ProjetRepository>();
builder.Services.AddScoped<IRepository<Lien>, LienRepository>();
builder.Services.AddScoped<IRepository<Contact>, ContactRepository>();
builder.Services.AddScoped<IRepository<Experience>, ExperienceRepository>();
builder.Services.AddScoped<IRepositoryPortfolio<UtilisateurDTO>, PortfolioRepository>();


// Service
builder.Services.AddScoped<UserLoginService>();
builder.Services.AddScoped<UtilisateurService>();
builder.Services.AddScoped<ProjetService>();
builder.Services.AddScoped<LienService>();
builder.Services.AddScoped<ContactService>();
builder.Services.AddScoped<ProtectedLocalStorage>();

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
    //client.BaseAddress = new Uri("https://localhost:7047/");
    client.BaseAddress = new Uri("https://portofolioapi-8nmz.onrender.com");
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


app.Run();