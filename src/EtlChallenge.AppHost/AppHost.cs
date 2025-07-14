var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.EltChallenge_UI>("ui");
builder.AddProject<Projects.EtlChallenge_FileUploadService>("unzip-service");
builder.AddProject<Projects.EtlChallenge_LoadService>("load-service");
builder.AddProject<Projects.EtlChallenge_ParserService>("parser-service");
builder.AddProject<Projects.EtlChallenge_ValidateService>("validate-service");

await builder.Build().RunAsync();
