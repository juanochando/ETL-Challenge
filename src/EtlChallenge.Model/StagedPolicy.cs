using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace EtlChallenge.Model;

[ExcludeFromCodeCoverage]
public class StagedPolicy
{
    [Key]
    public required string Id { get; set; }

    public required Guid CorrelationId { get; set; }

    public required string Name { get; set; }

    public required string FileStorageReference { get; set; }
}
