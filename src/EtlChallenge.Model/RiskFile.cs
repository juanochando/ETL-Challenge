using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace EtlChallenge.Model;

[ExcludeFromCodeCoverage]
public class RiskFile
{
    /// <summary>
    /// Reference for locating the risks file in the storage.
    /// </summary>
    [Key]
    public required string StorageReference { get; set; }

    public ICollection<Risk> Risks { get; set; } = [];
}
