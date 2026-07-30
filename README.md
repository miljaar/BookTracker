dotnet test start een tijdelijke PostgreSQL-container.
Docker Desktop of Docker Engine moet actief zijn.

## Run slow tests
run tests voor database- en endpointwerk.
De tests starten een tijdelijke PostgreSQL-container en vereisen Docker.

```bash
dotnet test BookTracker.Api.IntegrationTests/BookTracker.Api.IntegrationTests.csproj
```

## Run fast tests
run tests voor gewone domain- en applicatieontwikkeling. Deze hebben geen docker nodig
```bash
dotnet test BookTracker.Api.Tests/BookTracker.Api.Tests.csproj
```

## Run alle tests
```bash
dotnet test BookTracker.sln
```