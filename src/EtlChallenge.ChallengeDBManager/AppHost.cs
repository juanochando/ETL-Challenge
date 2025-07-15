using Microsoft.EntityFrameworkCore;
using EtlChallenge.ChallengeDB;
using EtlChallenge.ChallengeDBManager;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddSqlServerDbContext<ChallengeDBContext>("etlchallengedatabase", null,
    optionsBuilder => optionsBuilder.UseSqlServer(sqlServerBuilder =>
    sqlServerBuilder.MigrationsAssembly(typeof(ChallengeDBContext).Assembly.GetName().Name))
);

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(ChallengeDBInitializer.ActivitySourceName));

builder.Services.AddSingleton<ChallengeDBInitializer>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<ChallengeDBInitializer>());
builder.Services.AddHealthChecks()
    .AddCheck<ChallengeDBInitializerHealthCheck>("DbInitializer", null);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapPost(
        "/reset-db",
        async (ChallengeDBContext dbContext, ChallengeDBInitializer dbInitializer, CancellationToken cancellationToken) =>
        {
            // Delete and recreate the database. This is useful for development scenarios to reset the database to its initial state.
            await dbContext.Database.EnsureDeletedAsync(cancellationToken);
            await dbInitializer.InitializeDatabaseAsync(dbContext, cancellationToken);
        });
}

app.MapDefaultEndpoints();

await app.RunAsync();
