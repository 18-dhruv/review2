namespace SubDomainAnalyser2;

[TestFixture]
public class DomainAnalyzerTests
{
    private DomainAnalyzer<DomainRecord> analyzer = null!;

    [SetUp]
    public void Setup()
    {
        analyzer = new DomainAnalyzer<DomainRecord>
        {
            MaxDepth = 4
        };
    }

    [Test]
    public void AddDomain_WhenLabelStartsWithHyphen_ReturnsFalse()
    {
        var result = analyzer.AddDomain("-api.example.com");

        Assert.That(result, Is.False);
        Assert.That(analyzer.ValidDomains, Is.Empty);
        Assert.That(analyzer.InvalidDomains, Contains.Item("-api.example.com"));
    }

    [Test]
    public void AddDomain_WhenLabelEndsWithHyphen_ReturnsFalse()
    {
        var result = analyzer.AddDomain("api-.example.com");

        Assert.That(result, Is.False);
        Assert.That(analyzer.InvalidDomains, Contains.Item("api-.example.com"));
    }

    [Test]
    public void AddDomain_WhenLabelIsEmpty_ReturnsFalse()
    {
        var result = analyzer.AddDomain("api..example.com");

        Assert.That(result, Is.False);
        Assert.That(analyzer.InvalidDomains, Contains.Item("api..example.com"));
    }

    [Test]
    public void AddDomain_WhenLabelIsTooLong_ReturnsFalse()
    {
        var label = new string('a', 64);
        var domain = $"{label}.example.com";

        var result = analyzer.AddDomain(domain);

        Assert.That(result, Is.False);
    }

    [Test]
    public void AddDomain_WhenLabelHas63Characters_ReturnsTrue()
    {
        var label = new string('a', 63);
        var domain = $"{label}.example.com";

        var result = analyzer.AddDomain(domain);

        Assert.That(result, Is.True);
    }

    [Test]
    public void AddDomain_WhenDomainIsValid_AddsItToValidDomains()
    {
        var result = analyzer.AddDomain("api.eu.payments.example.co.uk");

        Assert.That(result, Is.True);
        Assert.That(analyzer.ValidDomains, Has.Count.EqualTo(1));

        var domain = analyzer.ValidDomains[0];

        Assert.That(domain.RawDomain, Is.EqualTo("api.eu.payments.example.co.uk"));

        Assert.That(domain.Labels, Is.EqualTo(new List<string>
        {
            "api",
            "eu",
            "payments",
            "example",
            "co",
            "uk"
        }));

        Assert.That(domain.Depth, Is.EqualTo(6));

        Assert.That(domain.HierarchyRightToLeft, Is.EqualTo(new List<string>
        {
            "uk",
            "co",
            "example",
            "payments",
            "eu",
            "api"
        }));
    }

    [Test]
    public void GetDeepDomains_WhenDomainIsDeeperThanMaxDepth_ReturnsIt()
    {
        analyzer.MaxDepth = 4;

        analyzer.AddDomain("api.eu.payments.example.co.uk");
        analyzer.AddDomain("example.com");

        var deepDomains = analyzer.GetDeepDomains();

        Assert.That(deepDomains, Has.Count.EqualTo(1));
        Assert.That(
            deepDomains[0].RawDomain,
            Is.EqualTo("api.eu.payments.example.co.uk")
        );
    }

    [Test]
    public void GetDeepDomains_WhenNoDomainIsTooDeep_ReturnsEmpty()
    {
        analyzer.MaxDepth = 6;

        analyzer.AddDomain("api.eu.payments.example.co.uk");

        var deepDomains = analyzer.GetDeepDomains();

        Assert.That(deepDomains, Is.Empty);
    }

    [Test]
    public void IsSuspiciousLabel_WhenLabelLooksRandom_ReturnsTrue()
    {
        var result = analyzer.IsSuspiciousLabel("xj39qk7mz81p");

        Assert.That(result, Is.True);
    }

    [Test]
    public void IsSuspiciousLabel_WhenLabelIsReadable_ReturnsFalse()
    {
        var result = analyzer.IsSuspiciousLabel("payments");

        Assert.That(result, Is.False);
    }

    [Test]
    public void GetSuspiciousDomainsByRisk_ReturnsDomainsInRiskOrder()
    {
        analyzer.AddDomain("xj39qk7mz81p.example.com");
        analyzer.AddDomain("a1b2c3d4e5f6g7h8.example.com");
        analyzer.AddDomain("payments.example.com");

        var domains = analyzer.GetSuspiciousDomainsByRisk();

        Assert.That(domains, Is.Not.Empty);

        for (var i = 0; i < domains.Count - 1; i++)
        {
            var currentRisk = analyzer.CalculateDomainRiskScore(domains[i]);
            var nextRisk = analyzer.CalculateDomainRiskScore(domains[i + 1]);

            Assert.That(currentRisk, Is.GreaterThanOrEqualTo(nextRisk));
        }
    }

    [Test]
    public void GetMostFrequentLabels_ReturnsMostCommonLabel()
    {
        analyzer.AddDomain("api.example.com");
        analyzer.AddDomain("cdn.example.org");
        analyzer.AddDomain("mail.example.net");

        var labels = analyzer.GetMostFrequentLabels(1);

        Assert.That(labels, Has.Count.EqualTo(1));
        Assert.That(labels[0].Label, Is.EqualTo("example"));
        Assert.That(labels[0].Count, Is.EqualTo(3));
    }

    [Test]
    public void GetSummary_ReturnsCorrectCounts()
    {
        analyzer.MaxDepth = 3;

        analyzer.AddDomain("api.example.com");
        analyzer.AddDomain("api.eu.payments.example.co.uk");
        analyzer.AddDomain("-bad.example.com");
        analyzer.AddDomain("xj39qk7mz81p.example.com");

        var summary = analyzer.GetSummary();

        Assert.That(summary.TotalValidDomains, Is.EqualTo(3));
        Assert.That(summary.TotalInvalidDomains, Is.EqualTo(1));
        Assert.That(summary.DeepDomainCount, Is.EqualTo(1));
        Assert.That(summary.SuspiciousLabelCount, Is.GreaterThanOrEqualTo(1));
    }
}