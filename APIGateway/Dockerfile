# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY APIGateway/APIGateway.csproj APIGateway/
RUN dotnet restore APIGateway/APIGateway.csproj

COPY . .
WORKDIR /src/APIGateway
RUN dotnet publish APIGateway.csproj -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "APIGateway.dll"]