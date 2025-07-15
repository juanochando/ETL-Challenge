using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using EtlChallenge.Application.Services;
using EtlChallenge.StorageService.Extensions;

namespace EtlChallenge.Application.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEtlChallengeApplication(
            this IServiceCollection services,
            IConfiguration configuration,
            Type[]? consumers = null)
    {
        services.AddStorageService(configuration);

        services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();

                x.UsingRabbitMq((context, cfg) =>
                {
                    var host = configuration.GetConnectionString("RabbitMQConnection");
                    cfg.Host(host);
                    cfg.ConfigureEndpoints(context);
                });

                if (consumers is not null)
                    x.AddConsumers(consumers);
            });

        services.AddTransient<IPolicyService, PolicyService>();

        return services;
    }
}
