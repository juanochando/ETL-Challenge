var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();

var app = builder.Build();

await app.RunAsync();
