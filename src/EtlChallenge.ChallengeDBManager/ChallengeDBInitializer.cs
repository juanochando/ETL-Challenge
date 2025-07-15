using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using EtlChallenge.ChallengeDB;

namespace EtlChallenge.ChallengeDBManager;

internal class ChallengeDBInitializer(
    IServiceProvider serviceProvider,
    ILogger<ChallengeDBInitializer> logger) : BackgroundService
{
    public const string ActivitySourceName = "Migrations";

    private readonly ActivitySource _activitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ChallengeDBContext>();

        using var activity = _activitySource.StartActivity("Initializing challenge database", ActivityKind.Client);
        await InitializeDatabaseAsync(dbContext, cancellationToken);
    }

    public async Task InitializeDatabaseAsync(ChallengeDBContext dbContext, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(dbContext.Database.MigrateAsync, cancellationToken);

        await SeedAsync(dbContext, cancellationToken);

        logger.LogInformation("Database initialization completed after {ElapsedMilliseconds}ms", sw.ElapsedMilliseconds);
    }

    private async Task SeedAsync(ChallengeDBContext dbContext, CancellationToken cancellationToken)
    {
        logger.LogInformation("Seeding database");

        // Simulate asynchronous operation
        await Task.Delay(100, cancellationToken);
    }
}
