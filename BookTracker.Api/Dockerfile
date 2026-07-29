# Build stage: de SDK bevat restore-, build- en publishtools.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Kopieer eerst alleen het projectbestand. Deze restorelaag blijft gecachet
# zolang de NuGet-dependencies niet veranderen.
COPY BookTracker.Api/BookTracker.Api.csproj BookTracker.Api/
RUN dotnet restore BookTracker.Api/BookTracker.Api.csproj

# Kopieer daarna de broncode en maak een Release-publicatie.
COPY BookTracker.Api/ BookTracker.Api/
RUN dotnet publish BookTracker.Api/BookTracker.Api.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore

# Runtime stage: kleiner image zonder SDK en compiler.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Neem alleen het gepubliceerde resultaat uit de build stage over.
COPY --from=build /app/publish .

# Start de API zonder rootrechten en luister op de .NET 8-containerpoort.
USER app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

# Dit proces wordt gestart wanneer de container begint.
ENTRYPOINT ["dotnet", "BookTracker.Api.dll"]