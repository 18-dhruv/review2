namespace SubDomainAnalyser2;


public class DomainRecord : IDomainRecord
{
    public string RawDomain{ get; set; }= "";
    public List<string> Labels { get; set; } =new List<string>();
    public int Depth => Labels.Count;
    public List<string> HierarchyRightToLeft
    {
        get
        {
            var stack = new Stack<string>(Labels);
            return stack.ToList();
        }
    }
    public override string ToString() => RawDomain;
    
    public DomainRecord(){}
    
}