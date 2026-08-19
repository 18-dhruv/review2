namespace SubdomainAnalyser;

public class Tree
{ 
    public Node head;
    public Node left;
    public Node right;
    public Tree(Node head)
    {
       this.head = head;
       this.left = null;
       this.right = null;
    }
    
}