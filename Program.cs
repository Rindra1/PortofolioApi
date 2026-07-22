using PortofolioApi.Components;
using PortofolioApi.Domain.Entities;
using PortofolioApi.Domain.DTOs;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

using Microsoft.EntityFrameworkCore;
using PortofolioApi.Application.Services;
using PortofolioApi.Infrastructure.Data;
using PortofolioApi.Configuration;
using PortofolioApi.Domain.Interfaces;
using PortofolioApi.Services;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using System.Net;
using Microsoft.AspNetCore.ResponseCompression;

using Microsoft.AspNetCore.StaticFiles;


var builder = WebApplication.CreateBuilder(args);

// Chemin vers la base, compatible local et Render
var dbPath = Path.Combine(
    builder.Environment.ContentRootPath,
    "Data",
    "appdata.db");

// Téléchargement si la base n'existe pas
if (!File.Exists(dbPath))
{
    using var client = new WebClient();
    try
    {
        client.DownloadFile(
            "https://github.com/Rindra1/PortofolioApi/raw/refs/heads/main/appdata.db",
            dbPath
        );
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erreur lors du téléchargement de la base : {ex.Message}");
    }
}

// Ajoute cette ligne juste après la création du builder
builder.Configuration.AddEnvironmentVariables();

// Liaison du fichier appsettings.json ou des variables Render
builder.Services.Configure<SendGridSettings>(
    builder.Configuration.GetSection("SendGridSettings")
);

builder.Services.AddScoped<UserState>();
builder.Services.AddScoped<PortofolioApi.Services.TokenServices>();

// Localization service (FR/EN)
builder.Services.AddScoped<PortofolioApi.Services.LocalizationService>();

// Bind JwtSettings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Add TokenService
builder.Services.AddScoped<ITokenService, TokenService>();

// Add HttpContext accessor & CurrentUserService (optionnel)
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
if (jwtSettings is null || string.IsNullOrWhiteSpace(jwtSettings.Secret))
{
    throw new InvalidOperationException("JwtSettings:Secret doit etre defini via les variables d'environnement ou user-secrets.");
}

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
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

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
builder.Services.AddScoped<IRepository<Competence>, CompetenceRepository>();


// Ajouter les services API et HttpClient
builder.Services.AddEndpointsApiExplorer();



builder.Services.AddHttpClient("API", client =>
{
    //client.BaseAddress = new Uri("http://localhost:5000/"); //En local
    client.BaseAddress = new Uri("https://rindra-dotnet-developer.onrender.com"); //Sur Render
    client.Timeout = TimeSpan.FromSeconds(60); // éviter le TaskCanceled
});

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));

// Ajouter les services Swagger
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
});

// Ajouter les services Blazor Server et Razor Components
builder.Services.AddRazorPages();
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);
});

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

builder.Services.AddMemoryCache();
builder.Services.AddResponseCaching();

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] {
            "text/css",
            "application/javascript",
            "application/json",
            "text/html",
            "image/svg+xml",
            "font/woff2",
            "font/woff"
        });
});

var app = builder.Build();

app.UseResponseCaching();

// Pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseHsts();

app.UseResponseCompression();

//Mise en cache
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        var path = ctx.File.PhysicalPath;

        if (path == null) return;

        // ===== 1. IMAGES - Cache long (1 an) =====
        var imageExtensions = new[] {
            ".jpg", ".jpeg", ".png", ".gif", ".bmp",
            ".webp", ".svg", ".ico", ".avif", ".tiff"
        };

        if (imageExtensions.Any(e => path.EndsWith(e, StringComparison.OrdinalIgnoreCase)))
        {
            // Cache d'1 an pour les images
            ctx.Context.Response.Headers.Append(
                "Cache-Control",
                "public, max-age=31536000, immutable"
            );

            // Ajouter un ETag pour la validation
            try
            {
                var etag = $"\"{Convert.ToBase64String(
                    System.Security.Cryptography.SHA1.HashData(
                        System.IO.File.ReadAllBytes(path)
                    )).Substring(0, 16)}\"";
                ctx.Context.Response.Headers.Append("ETag", etag);
            }
            catch
            {
                // Ignorer les erreurs de lecture
            }

            // Ajouter le type MIME correct
            var contentType = GetContentType(path);
            if (!string.IsNullOrEmpty(contentType))
            {
                ctx.Context.Response.Headers.Append("Content-Type", contentType);
            }

            // Ajouter un header de cache supplémentaire
            ctx.Context.Response.Headers.Append(
                "Expires",
                DateTime.UtcNow.AddYears(1).ToString("R")
            );
        }

        // ===== 2. CSS & JS - Cache long =====
        if (path.EndsWith(".css") ||
            path.EndsWith(".min.css") ||
            path.EndsWith(".js") ||
            path.EndsWith(".min.js"))
        {
            ctx.Context.Response.Headers.Append(
                "Cache-Control",
                "public, max-age=31536000, immutable"
            );
        }

        // ===== 3. Webmanifest - Cache moyen =====
        if (path.EndsWith(".webmanifest"))
        {
            ctx.Context.Response.Headers.Append(
                "Cache-Control",
                "public, max-age=3600" // 1 heure
            );
        }

        // ===== 4. HTML - Pas de cache =====
        if (path.EndsWith(".html") || path.EndsWith(".htm"))
        {
            ctx.Context.Response.Headers.Append(
                "Cache-Control",
                "no-cache, no-store, must-revalidate"
            );
            ctx.Context.Response.Headers.Append("Pragma", "no-cache");
            ctx.Context.Response.Headers.Append("Expires", "0");
        }
    }
});

// Fonction helper pour déterminer le type MIME
static string GetContentType(string path)
{
    var provider = new FileExtensionContentTypeProvider();
    if (provider.TryGetContentType(path, out string contentType))
    {
        return contentType;
    }
    return "application/octet-stream";
}

app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthentication();    // 🔹 Toujours avant Authorization
app.UseAuthorization();
app.UseAntiforgery();

if (app.Environment.IsDevelopment())
{
    app.MapGet("/routes", async context =>
    {
        var dataSource = context.RequestServices.GetRequiredService<EndpointDataSource>();
        var routes = dataSource.Endpoints
            .OfType<RouteEndpoint>()
            .Select(e => e.RoutePattern.RawText);

        await context.Response.WriteAsync("Endpoints:\n" + string.Join("\n", routes));
    });

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Portofolio API V1");
    });
}
// Routing API et Razor
app.MapControllers();

app.Use(async (context, next) =>
{
    var headers = context.Response.Headers;

    // COOP / COEP / CORP
    headers["Cross-Origin-Opener-Policy"] = "same-origin";
    headers["Cross-Origin-Embedder-Policy"] = "credentialless";
    headers["Cross-Origin-Resource-Policy"] = "cross-origin";

    // CSP avec Trusted Types
    headers["Content-Security-Policy"] =
        "default-src 'self';" +
        "base-uri 'self';" +
        "object-src 'none';" +
        "frame-ancestors 'none';" +
        "form-action 'self';" +
        "upgrade-insecure-requests;" +
        "script-src 'self' 'unsafe-inline' https://www.googletagmanager.com https://www.google-analytics.com https://www.gstatic.com https://cdnjs.cloudflare.com;" +
        "style-src 'self' 'unsafe-inline' https://cdnjs.cloudflare.com https://fonts.googleapis.com;" +
        "font-src 'self' https://cdnjs.cloudflare.com https://fonts.gstatic.com data:;" +
        "img-src 'self' data: blob: https:;" +
        "connect-src 'self' https://www.google-analytics.com https://region1.google-analytics.com wss: https:;" +
        "frame-src 'self' https://www.googletagmanager.com;" +
        "manifest-src 'self';" +
        "worker-src 'self' blob:;" +
        "media-src 'self' https:;" +
        "trusted-types default blazor blazor#bundler gtm;" +
        "require-trusted-types-for 'script';";


    // Content Security Policy
    headers["Content-Security-Policy"] =
        "default-src 'self';" +
        "base-uri 'self';" +
        "object-src 'none';" +
        "frame-ancestors 'none';" +
        "form-action 'self';" +
        "upgrade-insecure-requests;" +

        // Scripts (Blazor + GTM + Analytics + scripts inline de ton App.razor)
        "script-src 'self' 'unsafe-inline' https://www.googletagmanager.com https://www.google-analytics.com https://www.gstatic.com https://cdnjs.cloudflare.com;" +

        // Styles (AOS + FontAwesome + styles inline)
        "style-src 'self' 'unsafe-inline' https://cdnjs.cloudflare.com https://fonts.googleapis.com;" +

        // Polices
        "font-src 'self' https://cdnjs.cloudflare.com https://fonts.gstatic.com data:;" +

        // Images (OpenGraph, data URI, CDN HTTPS)
        "img-src 'self' data: blob: https:;" +

        // Connexions (Blazor Server utilise WebSocket)
        "connect-src 'self' https://www.google-analytics.com https://region1.google-analytics.com wss: https:;" +

        // Frames
        "frame-src 'self' https://www.googletagmanager.com;" +

        // Manifest PWA
        "manifest-src 'self';" +

        // Workers éventuels
        "worker-src 'self' blob:;" +

        // Médias
        "media-src 'self' https:;";

    // HSTS (au cas où le proxy ne le fait pas)
    headers["Strict-Transport-Security"] =
        "max-age=31536000; includeSubDomains; preload";

    // En-têtes complémentaires
    headers["X-Content-Type-Options"] = "nosniff";
    headers["X-Frame-Options"] = "DENY";
    headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    headers["Permissions-Policy"] =
        "camera=(), microphone=(), geolocation=(), payment=(), usb=()";

    // Cache des ressources statiques
    if (context.Request.Path.StartsWithSegments("/images") ||
        context.Request.Path.StartsWithSegments("/assets") ||
        context.Request.Path.StartsWithSegments("/_framework") ||
        context.Request.Path.StartsWithSegments("/lib"))
    {
        headers["Cache-Control"] = "public,max-age=31536000,immutable";
    }

    await next();
});


app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapGet("/sitemap.xml", async () =>
{
    var urls = new[]
    {
        "/", "/about", "/projets", "/experience", "/services", "/contact"
    };
});

app.Run();
