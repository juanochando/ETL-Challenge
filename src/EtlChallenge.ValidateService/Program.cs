using EtlChallenge.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();

builder.Services.AddEtlChallengeApplication(builder.Configuration);

var app = builder.Build();

await app.RunAsync();
