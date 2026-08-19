namespace SubDomainAnalyser2;

public interface IDomainRecord
{
    string RawDomain { get; set; }
    List<string> Labels { get; set; }
    int Depth { get; }
}