namespace SubdomainAnalyser;

public class Node
{
    public string Data;
    public Node Next;

    public Node(string data, Node next)
    {
        this.Data = data;
        this.Next = next;
    }
}
