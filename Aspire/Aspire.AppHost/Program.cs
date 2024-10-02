var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.API>("api");

builder.AddProject<Projects.BlazorWASM>("BlazorWASM");

builder.Build().Run();
