namespace Review4.models;

public class Book
{
    public string BookName { get;}
    public string Author { get; }
    public int isbn { get; }
    public Node<BorrowingHistory> BorrowingHistory;
    public Queue<Member> reservation;
    public Status BookStatus;
    public Book(string bookName, string author, int isbn)
    {
        this.BookName = bookName;
        this.Author = author;
        this.isbn = isbn;
        this.BorrowingHistory = null;
        this.BookStatus = Status.notBorrowed;
        this.reservation = new Queue<Member>();
    }
}

public enum Status
{
    Borrowed,
    notBorrowed
}