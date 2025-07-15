using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace EtlChallenge.Model;

[ExcludeFromCodeCoverage]
public class Policy
{
    [Key]
    public required string Id { get; set; }
    public required string Name { get; set; }

    public ICollection<Risk> Risks { get; set; } = [];

    public PolicyFile? PolicyFile { get; set; }
}
