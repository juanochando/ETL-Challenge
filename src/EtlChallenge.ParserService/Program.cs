using EtlChallenge.Application.Extensions;
using EtlChallenge.Application.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();

builder.Services.AddEtlChallengeApplication(
    builder.Configuration,
    [
        typeof(NewPolicyFileHandler),
        typeof(NewRiskFileHandler)
    ]);

var app = builder.Build();

await app.RunAsync();
