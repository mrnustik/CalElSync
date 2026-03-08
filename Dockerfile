FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY CalElSync/CalElSync.csproj CalElSync/
COPY CalElSync.Events.iCal/CalElSync.Events.iCal.csproj CalElSync.Events.iCal/
COPY CalElSync.Tasks.Todoist/CalElSync.Tasks.Todoist.csproj CalElSync.Tasks.Todoist/
COPY CalElSync.Configuration.Table/CalElSync.Configuration.Table.csproj CalElSync.Configuration.Table/
COPY CalElSync.Host/CalElSync.Host.csproj CalElSync.Host/

RUN dotnet restore CalElSync.Host/CalElSync.Host.csproj

COPY CalElSync/ CalElSync/
COPY CalElSync.Events.iCal/ CalElSync.Events.iCal/
COPY CalElSync.Tasks.Todoist/ CalElSync.Tasks.Todoist/
COPY CalElSync.Configuration.Table/ CalElSync.Configuration.Table/
COPY CalElSync.Host/ CalElSync.Host/

RUN dotnet publish CalElSync.Host/CalElSync.Host.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CalElSync.Host.dll"]
