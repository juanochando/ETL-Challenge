# ETL Challenge - AI Coding Agent Instructions

## Project Architecture

This is an event-driven ETL (Extract, Transform, Load) pipeline built using .NET 8 microservices with a choreography-based Saga pattern. The project uses .NET Aspire for orchestration during development.

### Key Components:

1. **AppHost**: Orchestrates services using .NET Aspire, configuring dependencies like SQL Server, RabbitMQ, and Azure Storage.

2. **Microservices**:

   - **ParserService**: Processes files and emits events for each record.
   - **ValidateService**: Validates records and emits success/failure events.
   - **LoadService**: Loads validated data into the database.
   - **ChallengeDBManager**: Manages database initialization.
   - **UI**: Provides user interface for the ETL pipeline.

3. **Event-Driven Communication**: Services communicate asynchronously via integration events using MassTransit and RabbitMQ.

4. **Models**:
   - `Policy`: Container for risks with ID, Name, and storage reference.
   - `Risk`: Contains risk details including Peril type, linked to a Policy.
   - `StagedPolicy`/`StagedRisk`: Used for temporary storage during ETL process.

## Development Workflow

### Building and Running

```bash
# Build the solution
dotnet build /workspaces/ETLChallenge/src/EtlChallenge.sln

# Run with Aspire (launches all services with dashboard)
cd /workspaces/ETLChallenge/src/EtlChallenge.AppHost
dotnet run
```

### Database Reset

Access `http://localhost:<port>/reset-db` from the db-manager service to reset the database.

## Project Patterns and Conventions

### Event Pattern

1. All events carry a `CorrelationId` for tracking the ETL job through the pipeline.
2. Event handlers are registered in each service's `Program.cs` file.
3. Events are defined in the `EtlChallenge.Contracts` project.

Example:

```csharp
// Publishing an event
await publishEndpoint.Publish(new PolicyParsedEvent(
    context.Message.CorrelationId,
    context.Message.PolicyFileReference,
    new Policy { /* properties */ }
));

// Consuming an event
public class LoadPolicyParsedHandler : IConsumer<PolicyParsedEvent>
{
    public async Task Consume(ConsumeContext<PolicyParsedEvent> context)
    {
        // Handle the event
    }
}
```

### Error Handling

- Services use try/catch blocks to handle exceptions.
- Errors are logged and published as failure events.
- For production, implement a circuit breaker and retry policies with Polly.

### Database Access

- Entity Framework Core is used via `ChallengeDBContext`.
- Staged entities are used for temporary storage during validation.
- Final validated entities are stored in the main tables.

## Integration and Dependencies

- **MassTransit**: Used for implementing the message queue consumers/publishers.
- **Azure Storage**: Used for blob storage of ETL files.
- **SQL Server**: Used for data persistence.
- **OpenTelemetry**: Used for tracing and metrics.

## Common Pitfalls and Tips

- Always maintain the `CorrelationId` when publishing new events.
- The AppHost sets up all dependencies; modify it when adding new services.
- Consider idempotency when handling events as they may be delivered multiple times.
- File parsing assumes tab-separated values with headers in the first line.
