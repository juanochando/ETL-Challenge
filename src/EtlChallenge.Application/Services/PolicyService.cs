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

        // TODO: Refactor to handle sepratedly policies and risks concerns, make each class and method single responsibility
        try
        {
            // Unzip file in memory
            using var zipArchive = new ZipArchive(rawPolicyFile, ZipArchiveMode.Read);

            logger.LogTrace("Unzipped raw policy file");

            // Validate the zip archive contains exactly two files: one for policies and one for risks
            if (zipArchive.Entries.Count != 2)
            {
                logger.LogError("Expected exactly two files in the zip archive, found {Count}", zipArchive.Entries.Count);
                throw new InvalidOperationException("Expected exactly two files in the zip archive.");
            }

            if (!zipArchive.Entries.Any(entry => entry.Name.Contains("policies", StringComparison.OrdinalIgnoreCase)))
            {
                logger.LogError("The zip archive does not contain a policies file");
                throw new InvalidOperationException("The file does not contain a policies file.");
            }

            if (!zipArchive.Entries.Any(entry => entry.Name.Contains("risks", StringComparison.OrdinalIgnoreCase)))
            {
                logger.LogError("The zip archive does not contain a risks file");
                throw new InvalidOperationException("The file does not contain a risks file.");
            }

            // TODO: Transactional behaviour, use a unit of work pattern if necessary

            // Find policy file and upload it
            var policyFile = zipArchive.Entries.FirstOrDefault(entry => entry.Name.Contains("policies", StringComparison.OrdinalIgnoreCase));
            using var policyFileStream = policyFile?.Open();

            if (policyFileStream == null)
            {
                logger.LogError("Could not open the compressed policy file as stream");
                throw new InvalidOperationException("Policy file stream is null.");
            }

            var policyFileReference = await storageService.UploadFileAsync("policyFile", policyFileStream!);
            logger.LogTrace("Policy file uploaded with reference {Reference}", policyFileReference);

            // Find risk file and upload it
            var riskFile = zipArchive.Entries.FirstOrDefault(entry => entry.Name.Contains("risks", StringComparison.OrdinalIgnoreCase));
            using var riskFileStream = riskFile?.Open();

            if (riskFileStream == null)
            {
                logger.LogError("Could not open the compressed risk file as stream");
                throw new InvalidOperationException("Risk file stream is null.");
            }

            var riskFileReference = await storageService.UploadFileAsync("riskFile", riskFileStream!);
            logger.LogTrace("Risk file uploaded with reference {Reference}", riskFileReference);

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
