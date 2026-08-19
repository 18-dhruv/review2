namespace SubDomainAnalyser2;

public class DomainAnalysisSummary
{
    public int TotalValidDomains { get; set; }
    public int TotalInvalidDomains { get; set; }
    public int DeepDomainCount { get; set; }
    public int SuspiciousLabelCount { get; set; }

    public List<(string Label, int Count)> MostFrequentLabels { get; set; } = new List<(string Label, int Count)>();

    public override string ToString()
    {
        var topLabels = string.Join(
            ", ",
            MostFrequentLabels.Select(label => $"{label.Label}({label.Count})")
        );

        return $"Valid: {TotalValidDomains}, " +
               $"Invalid: {TotalInvalidDomains}, " +
               $"Deep: {DeepDomainCount}, " +
               $"SuspiciousLabels: {SuspiciousLabelCount}, " +
               $"TopLabels: [{topLabels}]";
    }
}