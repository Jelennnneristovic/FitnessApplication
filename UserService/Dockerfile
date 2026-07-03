# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Kopiraj .csproj fajlove (za restore, keširanje slojeva)
COPY UserService/UserServiceAPI.csproj UserService/
COPY UserServiceApplication/UserServiceApplication.csproj UserServiceApplication/
COPY UserServiceDomain/UserServiceDomain.csproj UserServiceDomain/
COPY UserServiceInfrastructure/UserServiceInfrastructure.csproj UserServiceInfrastructure/

# Restore zavisnosti (za API projekat, povlaci i reference)
RUN dotnet restore UserService/UserServiceAPI.csproj

# Kopiraj sav kod
COPY . .

# Publish (build + priprema za produkciju)
WORKDIR /src/UserService
RUN dotnet publish UserServiceAPI.csproj -c Release -o /app/publish

# Stage 2: Runtime (manji image, bez SDK)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Port na kom servis slusa unutar kontejnera
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "UserServiceAPI.dll"]