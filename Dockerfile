# 1️⃣ Image de base pour exécuter .NET
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

# 2️⃣ Image pour construire l'application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copier le fichier .csproj et restaurer les packages (cache Docker efficace)
COPY ["PortofolioApi.csproj", "./"]
RUN dotnet restore "./PortofolioApi.csproj"

# Copier tout le projet
COPY . .

# Publier en Release avec optimisation pour WASM + trimming
RUN dotnet publish "PortofolioApi.csproj" \
    -c Release \
    -o /app/publish \
    -p:BlazorWebAssemblyEnableLinking=true \
    -p:PublishTrimmed=true \
    -p:RunAOTCompilation=true

# 3️⃣ Image finale
FROM base AS final
WORKDIR /app

# Copier uniquement ce qui est publié
COPY --from=build /app/publish .

# Lancer l'application
ENTRYPOINT ["dotnet", "PortofolioApi.dll"]
