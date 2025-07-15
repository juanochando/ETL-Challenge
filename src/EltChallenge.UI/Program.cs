using EtlChallenge.Contracts.Application;
using EtlChallenge.Application.Extensions;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(
        Assembly.GetAssembly(typeof(EtlChallenge.Application.Extensions.ServiceCollectionExtensions))
        ?? Assembly.GetExecutingAssembly());
});

builder.Services.AddEtlChallengeApplication(builder.Configuration);

builder.Services.AddRazorPages();

builder.Services.AddHttpForwarderWithServiceDiscovery();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

await app.RunAsync();
