using EtlChallenge.StorageService;
using Microsoft.Extensions.Logging;
using System.IO.Compression;

namespace EtlChallenge.Application.Services;

public class PolicyService(
    IStorageService storageService,
    ILogger<PolicyService> logger) : IPolicyService
{
    public async Task<(string policyFileReference, string riskFileReference)>
    ProcessRawPolicyFile(Stream rawPolicyFile)
    {
        logger.LogTrace("Processing new raw policy file");

        try
        {
            // Unzip file in memory
            using var zipArchive = new ZipArchive(rawPolicyFile, ZipArchiveMode.Read);

            // Validate the zip archive contains exactly two files: one for policies and one for risks
            // TODO: Use result pattern
            if (zipArchive.Entries.Count != 2)
            {
                throw new InvalidOperationException("Expected exactly two files in the zip archive.");
            }

            if (!zipArchive.Entries.Any(entry => entry.Name.Contains("policies", StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("The file does not contain a policies file.");
            }

            if (!zipArchive.Entries.Any(entry => entry.Name.Contains("risks", StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("The file does not contain a risks file.");
            }

            // Find policy file and upload it
            var policyFile = zipArchive.Entries.FirstOrDefault(entry => entry.Name.Contains("policy", StringComparison.OrdinalIgnoreCase));
            using var policyFileStream = policyFile?.Open();
            var policyFileReference = await storageService.UploadFileAsync("policyFile", policyFileStream!);

            // Find risk file and upload it
            var riskFile = zipArchive.Entries.FirstOrDefault(entry => entry.Name.Contains("risk", StringComparison.OrdinalIgnoreCase));
            using var riskFileStream = riskFile?.Open();
            var riskFileReference = await storageService.UploadFileAsync("riskFile", riskFileStream!);

            // emit event with details of the processed file
            // TODO: Implement event emission logic

            logger.LogTrace("Raw policy file processed successfully");

            // Return references for the policy file and risk file
            // TODO: Use result pattern
            return (policyFileReference, riskFileReference);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing raw policy file");
            throw new InvalidOperationException("An error occurred while processing the raw policy file.", ex);
        }
    }
}
