using System.Diagnostics.CodeAnalysis;

namespace EtlChallenge.LoadService.Domain.Model;

[ExcludeFromCodeCoverage]
public class PolicyFile
{
    /// <summary>
    /// Reference for locating the policy file in the storage.
    /// </summary>
    public required string StorageReference { get; set; }

    public ICollection<Policy> Policies { get; set; } = [];
}
