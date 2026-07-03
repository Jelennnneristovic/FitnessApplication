# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Kopiraj .csproj fajlove
COPY NotificationServiceAPI/NotificationServiceAPI.csproj NotificationServiceAPI/
COPY NotificationServiceApplication/NotificationServiceApplication.csproj NotificationServiceApplication/
COPY NotificationServiceDomain/NotificationServiceDomain.csproj NotificationServiceDomain/

# Ako Notification ima i Infrastructure projekat, dodaj i tu liniju:
# COPY NotificationServiceInfrastructure/NotificationServiceInfrastructure.csproj NotificationServiceInfrastructure/

# Restore
RUN dotnet restore NotificationServiceAPI/NotificationServiceAPI.csproj

# Kopiraj sav kod
COPY . .

# Publish
WORKDIR /src/NotificationServiceAPI
RUN dotnet publish NotificationServiceAPI.csproj -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "NotificationServiceAPI.dll"]