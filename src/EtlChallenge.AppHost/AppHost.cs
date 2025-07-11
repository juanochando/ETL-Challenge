var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.EltChallenge_UI>("ui");
builder.AddProject<Projects.EtlChallenge_LoadService>("ui");
builder.AddProject<Projects.EtlChallenge_UnzipService>("ui");
builder.AddProject<Projects.EtlChallenge_ValidateService>("ui");

builder.Build().Run();
