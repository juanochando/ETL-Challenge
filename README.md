# ETL Pipeline Microservices – Reference Implementation

# Solution Overview

This solution implements an Extract-Transform-Load (ETL) pipeline as three independent microservices – Unzip, Validate, and Load – communicating through asynchronous integration events.

The design is event-driven and follows a choreography-based Saga pattern for coordination. In this Saga, each service performs its task and publishes an event that triggers the next service, without a central orchestrator.

The pipeline flow is as follows:
 * File Reception Service:
  Receives a raw zip file over an API endpoint, stores it and publishes an event to indicate a new raw file is available.
  Then, using the listen to self pattern, starts the extraction of the file's contents and publishes a policy file integration event and a risk file integration event with details, specially the same Correlation ID.
 * Parser service:
  PolicyParser and RiskParser run independently. Each parses its file into records and emits one event per record. Must maintain the Correlation ID in each event.
 * Validate Service:
  Two consumers validate policies and risks separately. When triggered, it validates each record (schema, required fields, data quality).
  It publishes either a success or failure event for each record, maintaining the Correlation ID.
 * Aggregator / Saga
  A sate machine Saga listens to events. The Correlation ID will act as overall Saga state key.
  It shall keep track of the parent / children relationship between policies and risks. It will publish either a batch completed or batch error event.
 * Load Service: Subscribed to batch completed events. It performs the final ETL step, saving into database. After successfully loading, it emits a load completed or load error event, also maintaining the Correlation ID.

## Choreography

A batch‑level saga ensures each stage acts autonomously but the overall process is controlled.
Each service is responsible for listening to specific events and performing actions, then publishing the next event.

* This yields a loosely coupled system where services only know about event contracts, not about the specific implementation of other stages.
* A Saga orchestrator can be easily implemented with MassTransit state machine
* The entire ETL process is eventually consistent
  e.g., the data is loaded after passing through asynchronous validation – which is acceptable for ETL pipelines.
* Correlation & Context
  All integration events carry a correlation ID to tie together events belonging to the same ETL job. This way, each service can log and identify the pipeline instance, and the distributed trace (in logs or observability tools) can show the end-to-end flow for a given ID. This correlation ID also allows tracking the saga state easily.

## Error Handling & Compensation

  The choreography pattern can handle failures by emitting failure events and having relevant services respond.

  For example, if validation fails, the ValidationFailed event could prompt the Unzip service to delete the extracted files. If loading fails, depending on the scenario, one could:
  (a) have the Load service retry internally,
  (b) emit LoadFailed and have an operator or a compensating service handle cleanup (like removing partial data or marking the ETL run as failed in a log), or
  (c) design the Load step to be fully transactional (e.g., bulk insert in a transaction that rolls back on failure).

  For complexity and brevity reasons, this implementation will log failures and publish failure events.

  *In a real scenario I would implement a circuit breaker and retry policies with Polly, for example, or try to control it in the stateful saga*.

## Reliability

Reliable Messaging with the Outbox Pattern

To ensure reliability and consistency in the event-driven workflow, each microservice employs the Transactional Outbox pattern. This guarantees that whenever a service processes data and produces an integration event, the event will not be lost even if the service or broker crashes at that moment. The key idea is to encapsulate state changes and event publication in a single database transaction.

A separate background Outbox Publisher runs within each service to forward Outbox events to the message broker (RabbitMQ). Several implementations can be made, with MassTransit or as a hosted service or periodic task with Hangfire for simplicity, or much better with exchanges for pub/sub: e.g., publish FileUnzipped to an exchange, which can route to one or more subscribers. In this implementation, each event has a single intended subscriber (next service in chain), but using exchanges allows flexibility *I would really use in a real scenario the Listen to Self pattern*.

## Duplicate events

If a service crashes after publishing but before marking the outbox record as processed, it might publish the same event again on restart, Therefore, consumers (downstream services) should handle idempotency.

Including an Event ID or using the Correlation ID to deduplicate is an easy way to handle this in simple scenarios. Designing the each step to be idempotent is a best practice in event-driven systems but may introduce the need to keep track of processed jobs, which also introduces its own challenges when considering a distributes system.

## Implementation Details

Each microservice is implemented in C# on .NET 8, leveraging modern practices like minimal hosting APIs, dependency injection, and asynchronous processing.

The services are structured with a focus on separation of concerns and maintainability, following a lightweight Clean Architecture approach.

## Solution Layout

A shared Contracts library defines the integration event classes (so that all services use the same event definitions and serialization format).

All microservice projects reside in a single repository for easy coordination (though they are decoupled, a monorepo is convenient for this reference).

Each service project has a clean internal structure – e.g., we separate the domain/models, the application logic (commands, handlers), and infrastructure (DB context, messaging) into folders.

*Docker Compose Orchestration:* I provided a docker-compose.yml that defines all the services and infrastructure containers for local or test deployment.

*Dev Container (VS Code):* For an easy onboarding dev experience, I included a DevContainer configuration. This allows developers to open the repository in a fully configured Docker container with all dependencies.

*.NET Aspire Orchestration:* I integrated .NET Aspire to orchestrate and monitor the microservices during development. .NET Aspire is a suite of tools and libraries that act as a developer-time orchestrator and dashboard for multi-service apps.

*Environment Configuration:* All sensitive or environment-specific settings (connection strings, RabbitMQ credentials, etc.) are provided via configuration files or environment variables. Docker Compose passes these in. I used ASP.NET configuration providers to bind these to strongly-typed settings classes.

*CI/CD Considerations:* Although not explicitly requested, it’s worth noting this structure supports containerized deployment easily – each service has a Dockerfile and can be deployed to Kubernetes or other container hosts. The Outbox and Saga ensure reliability in distributed environments as well.

## Testing and Quality Assurance

Automated tests are included for critical parts of the pipeline, albeit on a simplified, non exhaustive way. Each microservice can be tested in isolation (unit tests for its internal logic) and in integration (end-to-end tests with all services running, using Aspire’s test harness features.

Every service also has health checks and uses structured logging (with correlation IDs) to ensure we can monitor and diagnose issues quickly.

## Domain Modelling

I discarded full DDD modeling of the domain, going instead for POCO classes intended for easy Entity Framework implementation of the persistence layer.

Thus, some implementation leaks are intentional, like having an isolated table with a Primary Key attribute that would not be present on a non entity root class.

# Useful commands

* alias build="dotnet build /workspaces/ETLChallenge/src/EtlChallenge.sln"
* alias run="dotnet run --project /workspaces/ETLChallenge/src/EtlChallenge.AppHost/EtlChallenge.AppHost.csproj"
