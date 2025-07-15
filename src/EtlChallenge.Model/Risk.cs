using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace EtlChallenge.Model;

[ExcludeFromCodeCoverage]
public class Risk
{
    [Key]
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required Perils Peril { get; set; }
    public required string PolicyId { get; set; }
    public required string Street { get; set; }
    public string? CID { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}
