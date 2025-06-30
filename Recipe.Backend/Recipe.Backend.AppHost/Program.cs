var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.Recipe_Backend_API>("recipes-api");

builder.Build().Run();