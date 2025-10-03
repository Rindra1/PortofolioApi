# 1️⃣ Image de base pour exécuter l'application .NET
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

# 2️⃣ Image pour construire l'application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copier le fichier .csproj et restaurer les packages
COPY ["PortofolioApi.csproj", "./"]
RUN dotnet restore "./PortofolioApi.csproj"

# Copier tout le projet et publier
COPY . .
RUN dotnet publish "PortofolioApi.csproj" -c Release -o /app/publish

# 3️⃣ Image finale pour exécuter l'application
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Lancer l'application
ENTRYPOINT ["dotnet", "PortofolioApi.dll"]
