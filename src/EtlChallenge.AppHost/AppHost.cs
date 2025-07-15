using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var dbserver = builder
    .AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent);

var db = dbserver
    .AddDatabase("etlchallengedatabase");

var messaging = builder
    .AddRabbitMQ("RabbitMQConnection");

var storage = builder
    .AddAzureStorage("Storage");

if (builder.Environment.IsDevelopment())
{
    storage.RunAsEmulator();
}

var blobs = storage.AddBlobs("BlobConnection");

builder
    .AddProject<Projects.EtlChallenge_ChallengeDBManager>("db-manager")
    .WithReference(db)
    .WaitFor(db)
    .WithHttpHealthCheck("/health")
    .WithHttpCommand("/reset-db", "Reset Database", commandOptions: new() { IconName = "DatabaseLightning" });

builder
    .AddProject<Projects.EtlChallenge_LoadService>("load-service")
    .WithReference(messaging)
    .WithReference(db)
    .WaitFor(db)
    .WithReference(blobs)
    .WaitFor(blobs);

builder
    .AddProject<Projects.EtlChallenge_ParserService>("parser-service")
    .WithReference(messaging)
    .WithReference(blobs)
    .WaitFor(blobs);

builder
    .AddProject<Projects.EtlChallenge_ValidateService>("validate-service")
    .WithReference(messaging);

builder.AddProject<Projects.EltChallenge_UI>("ui")
    .WithReference(messaging)
    .WithReference(blobs)
    .WaitFor(blobs)
    .WithExternalHttpEndpoints();

await builder.Build().RunAsync();
