using MediatR;
namespace EtlChallenge.Application.Commands;

public record NewRawPolicyFile(Stream fileContents) : IRequest<NewRawPolicyFileResponse> { }
public record NewRawPolicyFileResponse(string policyFileReference, string riskFileReference) { }
