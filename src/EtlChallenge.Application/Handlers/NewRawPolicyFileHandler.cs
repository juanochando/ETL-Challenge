using Microsoft.Extensions.Logging;
using MediatR;
using MassTransit;
using EtlChallenge.Application.Services;
using EtlChallenge.Application.Commands;
using EtlChallenge.Contracts.Events.RiskFile;
using EtlChallenge.Contracts.Events.PolicyFile;

namespace EtlChallenge.Application.Handlers;

public class NewRawPolicyFileHandler(
    IPolicyService policyService,
    IPublishEndpoint publishEndpoint,
    ILogger<NewRawPolicyFileHandler> logger)
 : IRequestHandler<NewRawPolicyFile, NewRawPolicyFileResponse>
{
    public async Task<NewRawPolicyFileResponse> Handle(NewRawPolicyFile request, CancellationToken cancellationToken)
    {
        logger.LogTrace("Processing new raw policy file upload for a file of size {Size}", request.FileContents.Length);

        // Process the raw policy file and get references for the policy and risk files
        var (policyFileReference, riskFileReference) = await policyService.ProcessRawPolicyFile(request.FileContents);

        // Emit two events, one for each new file available
        await publishEndpoint.Publish(
            new PolicyFileAddedEvent(policyFileReference, request.CorrelationId),
            cancellationToken);

        await publishEndpoint.Publish(
            new RiskFileAddedEvent(riskFileReference, request.CorrelationId),
            cancellationToken);

        logger.LogTrace("New raw policy file processed successfully with policy file reference {PolicyFileReference} and risk file reference {RiskFileReference}",
            policyFileReference, riskFileReference);

        return new NewRawPolicyFileResponse(request.CorrelationId, policyFileReference, riskFileReference);
    }
}
