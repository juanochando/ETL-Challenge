using MediatR;
using EtlChallenge.Application.Services;
using EtlChallenge.Application.Commands;

namespace EtlChallenge.Application.Handlers;

public class NewRawPolicyFileHandler(IPolicyService policyService)
 : IRequestHandler<NewRawPolicyFile, NewRawPolicyFileResponse>
{
    public async Task<NewRawPolicyFileResponse> Handle(NewRawPolicyFile request, CancellationToken cancellationToken)
    {
        var (policyFileReference, riskFileReference) = await policyService.ProcessRawPolicyFile(request.fileContents);

        // emit two events, one for each new file available
        // TODO: Implement event emission logic here

        return new NewRawPolicyFileResponse(policyFileReference, riskFileReference);
    }
}
