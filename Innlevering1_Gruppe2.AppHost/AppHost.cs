using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

//lokale tilkoblingen uten å starte eller endre databasen.
var localSettings = Path.GetFullPath(Path.Combine(builder.AppHostDirectory, "../Reachout/appsettings.Local.json"));
builder.Configuration.AddJsonFile(localSettings, optional: true).AddEnvironmentVariables();

var database = builder.AddConnectionString("DefaultConnection");

builder.AddProject<Projects.Reachout>("reachout", launchProfileName: null)
    .WithReference(database)
    .WithHttpEndpoint(port: 5196, name: "http")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("DOTNET_USE_POLLING_FILE_WATCHER", "1");

builder.Build().Run();
