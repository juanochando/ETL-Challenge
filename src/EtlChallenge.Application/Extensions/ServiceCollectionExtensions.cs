using System.Diagnostics.CodeAnalysis;
using EtlChallenge.Application.Services;
using EtlChallenge.StorageService;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EtlChallenge.Application.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEtlChallengeApplication(
            this IServiceCollection services,
            IConfiguration configuration)
    {
        services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();

                x.UsingRabbitMq((context, cfg) =>
                {
                    var host = configuration.GetConnectionString("RabbitMQConnection");
                    cfg.Host(host);
                    cfg.ConfigureEndpoints(context);
                });
            });

        services.AddSingleton<IStorageService, StorageService.StorageService>();
        services.AddTransient<IPolicyService, PolicyService>();

        return services;
    }
}
