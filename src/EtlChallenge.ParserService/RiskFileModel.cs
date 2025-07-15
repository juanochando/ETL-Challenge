using EtlChallenge.Model;

namespace EtlChallenge.ParserService;

public class RiskFileModel
{
    public string ID { get; set; } = string.Empty;
    public string RiskName { get; set; } = string.Empty;
    public Perils Peril { get; set; }
    public string PolicyID { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string CID { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
