namespace SubDomainAnalyser2;


public class DomainAnalysisSummary
{
    public int TotalValidDomains { get; set; }
    public int TotalInvalidDomains { get; set; }
    public int DeepDomainCount { get; set; }
    public int SuspiciousLabelCount { get; set; }
    public List<(string Label, int Count)> MostFrequentLabels { get; set; } = new();

    public override string ToString() =>
        $"Valid: {TotalValidDomains}, Invalid: {TotalInvalidDomains}, " +
        $"Deep: {DeepDomainCount}, SuspiciousLabels: {SuspiciousLabelCount}, " +
        $"TopLabels: [{string.Join(", ", MostFrequentLabels.Select(l => $"{l.Label}({l.Count})"))}]";
}