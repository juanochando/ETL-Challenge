using EltChallenge.UI.Services;
using EtlChallenge.Contracts.Application;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddHttpForwarderWithServiceDiscovery();

builder.Services.AddHttpServiceReference<FileUploadServiceClient>(
    Constants.FileUploadServiceIdentifier,
    //"https+http://catalogservice",
    healthRelativePath: "health");

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

// app.MapForwarder("/catalog/images/{id}", "https+http://catalogservice", "/api/v1/catalog/items/{id}/image");


await app.RunAsync();
