using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using MassTransit;
using EtlChallenge.Application.Services;
using EtlChallenge.StorageService.Extensions;
using EtlChallenge.ChallengeDB;
using System.Reflection;

namespace EtlChallenge.Application.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEtlChallengeApplication(
            this IServiceCollection services,
            IConfiguration configuration,
            Type[]? consumers = null,
            Type[]? sagas = null)
    {
        services.AddStorageService(configuration);

        services.AddDbContext<ChallengeDBContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("etlchallengedatabase")
                    ?? throw new InvalidOperationException("Connection string 'etlchallengedatabase' not found.");
                options.UseSqlServer(connectionString);

            });

        services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();

                x.SetInMemorySagaRepositoryProvider();

                x.UsingRabbitMq((context, cfg) =>
                {
                    var host = configuration.GetConnectionString("RabbitMQConnection");
                    cfg.Host(host);
                    cfg.ConfigureEndpoints(context);
                });

                if (consumers is not null)
                    x.AddConsumers(consumers);

                if (sagas is not null)
                    Array.ForEach(
                        sagas,
                        saga => x.AddSagaStateMachine(saga));
            });

        services.AddTransient<IPolicyService, PolicyService>();

        return services;
    }
}
