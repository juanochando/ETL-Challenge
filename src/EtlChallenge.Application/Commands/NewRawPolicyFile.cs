using MediatR;
namespace EtlChallenge.Application.Commands;

public record NewRawPolicyFile(Guid CorrelationId, Stream FileContents) : IRequest<NewRawPolicyFileResponse> { }
public record NewRawPolicyFileResponse(Guid CorrelationId, string PolicyFileReference, string RiskFileReference) { }
