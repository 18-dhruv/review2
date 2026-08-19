using SubDomainAnalyser2;

var analyzer = new DomainAnalyzer<DomainRecord>() ;

var rawDomains = new List<string>
{
    "api.example.com",
    "cdn3.static.example.com",
    "api.eu.payments.example.co.uk",
    "-api.example.com",          
    "api-.example.com",          
    "api..example.com",         
    "xj39qk7mz81p.example.com",  
    "mail.example.com",
    "cdn.example.com"
};

analyzer.AddDomains(rawDomains);

Console.WriteLine("Summary");
Console.WriteLine(analyzer.GetSummary());

Console.WriteLine("Deep domains (depth > MaxDepth)");
foreach (var d in analyzer.GetDeepDomains())
{
    Console.WriteLine($"{d.RawDomain} (depth {d.Depth})");
}

Console.WriteLine("Suspicious domains by risk (descending)");
foreach (var d in analyzer.GetSuspiciousDomainsByRisk())
{
    Console.WriteLine($"{d.RawDomain} -> risk {analyzer.CalculateDomainRiskScore(d):F2}");
}

Console.WriteLine("Most frequent labels");
foreach (var (label, count) in analyzer.GetMostFrequentLabels())
{
    Console.WriteLine($"{label}: {count}");
}
//implemeted 
//NUnit  , regrex,linq 
//optimize 
// duplicte check in domain 

