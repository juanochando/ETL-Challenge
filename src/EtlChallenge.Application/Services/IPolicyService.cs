namespace EtlChallenge.Application.Services;

public interface IPolicyService
{
    /// <summary>
    /// Processes a raw policy file by extracting its contents,
    /// uploading the policy and risk files to storage,
    /// and returning their references.
    /// </summary>
    /// <param name="policyFileReference"></param>
    /// <param name="rawPolicyFile"></param>
    /// <returns></returns>
    Task<(string policyFileReference, string riskFileReference)> ProcessRawPolicyFile(Stream rawPolicyFile);
}
