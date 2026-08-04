FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY WorldCupScoreboard.sln ./
COPY src/WorldCupScoreboard/WorldCupScoreboard.csproj src/WorldCupScoreboard/
COPY src/WorldCupScoreboard.Api/WorldCupScoreboard.Api.csproj src/WorldCupScoreboard.Api/
RUN dotnet restore src/WorldCupScoreboard.Api/WorldCupScoreboard.Api.csproj

COPY src/WorldCupScoreboard/ src/WorldCupScoreboard/
COPY src/WorldCupScoreboard.Api/ src/WorldCupScoreboard.Api/
RUN dotnet publish src/WorldCupScoreboard.Api/WorldCupScoreboard.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "WorldCupScoreboard.Api.dll"]
