using System.Diagnostics.CodeAnalysis;
using EtlChallenge.Contracts.Application;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EtlChallenge.StorageService.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStorageService(
            this IServiceCollection services,
            IConfiguration configuration)
    {
        services.AddAzureClients(builder =>
        {
            builder.AddBlobServiceClient(configuration.GetConnectionString("BlobConnection"));
        });


        services.AddTransient<IStorageService, StorageService>();

        return services;
    }
}
