# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Kopiraj .csproj fajlove
COPY TrainingManagementAPI/TrainingManagementAPI.csproj TrainingManagementAPI/
COPY TrainingManagementApplication/TrainingManagementApplication.csproj TrainingManagementApplication/
COPY TrainingManagementDomain/TrainingManagementDomain.csproj TrainingManagementDomain/
COPY TrainingManagementInfrastructure/TrainingManagementInfrastructure.csproj TrainingManagementInfrastructure/

# Restore
RUN dotnet restore TrainingManagementAPI/TrainingManagementAPI.csproj

# Kopiraj sav kod
COPY . .

# Publish
WORKDIR /src/TrainingManagementAPI
RUN dotnet publish TrainingManagementAPI.csproj -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "TrainingManagementAPI.dll"]