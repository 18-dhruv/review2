namespace Review4.models;

public class Node<T>
{
    public T Data { get; set; }
    public Node<T> Next;
    public Node<T> Prev;

    public Node(T data)
    {
        this.Data = data;
        this.Next = null;
        this.Prev = null;
    }
}