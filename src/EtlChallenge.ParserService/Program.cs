using EtlChallenge.Application.Extensions;
using EtlChallenge.ParserService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();

builder.Services.AddEtlChallengeApplication(
    builder.Configuration,
    [typeof(NewPolicyFileHandler)]);

var app = builder.Build();

await app.RunAsync();
