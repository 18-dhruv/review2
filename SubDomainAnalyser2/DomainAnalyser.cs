namespace SubDomainAnalyser2;

public class DomainAnalyzer<T> where T : class, IDomainRecord, new()
{
    private readonly List<T> validDomains = new List<T>();
    private readonly List<string> invalidDomains = new List<string>();

    public int MaxDepth { get; set; } = 4;
    public double SuspicionThreshold { get; set; } = 0.65;

    public IReadOnlyList<T> ValidDomains => validDomains;
    public IReadOnlyList<string> InvalidDomains => invalidDomains;

    public bool AddDomain(string rawDomain)
    {
        if (!DomainValidator.Validate(rawDomain, out List<string> labels))
        {
            invalidDomains.Add(rawDomain);
            return false;
        }

        var domain = new T
        {
            RawDomain = rawDomain,
            Labels = labels
        };

        validDomains.Add(domain);
        return true;
    }

    public void AddDomains(IEnumerable<string> rawDomains)
    {
        foreach (var domain in rawDomains)
        {
            AddDomain(domain);
        }
    }

    public List<T> GetDeepDomains()
    {
        return validDomains
            .Where(domain => domain.Depth > MaxDepth)
            .ToList();
    }

    public double CalculateSuspicionScore(string label)
    {
        int length = label.Length;

        int vowelCount = label.Count(c => "aeiouAEIOU".Contains(c));
        int digitCount = label.Count(char.IsDigit);
        int distinctCount = label.Distinct().Count();

        double vowelRatio = (double)vowelCount / length;
        double digitRatio = (double)digitCount / length;
        double diversityRatio = (double)distinctCount / length;

        double entropy = label
            .GroupBy(c => c)
            .Select(group =>
            {
                double probability = (double)group.Count() / length;
                return -probability * Math.Log(probability, 2);
            })
            .Sum();

        double maxEntropy = Math.Log(length, 2);
        double normalizedEntropy =
            maxEntropy > 0 ? entropy / maxEntropy : 0;

        double score =
            (1 - vowelRatio) * 0.45 +
            digitRatio * 0.25 +
            diversityRatio * 0.15 +
            normalizedEntropy * 0.15;

        return Math.Round(Math.Clamp(score, 0, 1), 4);
    }

    public bool IsSuspiciousLabel(string label)
    {
        return CalculateSuspicionScore(label) >= SuspicionThreshold;
    }

    public List<string> GetSuspiciousLabels(T domain)
    {
        return domain.Labels
            .Where(IsSuspiciousLabel)
            .ToList();
    }

    public double CalculateDomainRiskScore(T domain)
    {
        return domain.Labels.Average(CalculateSuspicionScore);
    }

    public List<T> GetSuspiciousDomainsByRisk()
    {
        return validDomains
            .Where(domain => domain.Labels.Any(IsSuspiciousLabel))
            .OrderByDescending(CalculateDomainRiskScore)
            .ToList();
    }

    public List<(string Label, int Count)> GetMostFrequentLabels(int topN = 10)
    {
        return validDomains
            .SelectMany(domain => domain.Labels)
            .GroupBy(label => label, StringComparer.OrdinalIgnoreCase)
            .Select(group => (
                Label: group.Key,
                Count: group.Count()
            ))
            .OrderByDescending(x => x.Count)
            .ThenBy(x => x.Label, StringComparer.OrdinalIgnoreCase)
            .Take(topN)
            .ToList();
    }

    public DomainAnalysisSummary GetSummary()
    {
        return new DomainAnalysisSummary
        {
            TotalValidDomains = validDomains.Count,
            TotalInvalidDomains = invalidDomains.Count,
            DeepDomainCount = GetDeepDomains().Count,
            SuspiciousLabelCount = validDomains
                .SelectMany(domain => domain.Labels)
                .Count(IsSuspiciousLabel),
            MostFrequentLabels = GetMostFrequentLabels(5)
        };
    }
}