using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace EtlChallenge.Model;

[ExcludeFromCodeCoverage]
public class PolicyFile
{
    /// <summary>
    /// Reference for locating the policy file in the storage.
    /// </summary>
    [Key]
    public required string StorageReference { get; set; }

    public ICollection<Policy> Policies { get; set; } = [];
}
