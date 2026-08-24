using Review4.Exceptions;
using Review4.models;

namespace Review4.service;

public class BorrowingService
{
    public readonly ManagementService Manage;
    private Stack<KeyValuePair<Member, Book>>transaction;
    public BorrowingService(ManagementService manage)
    {
        this.Manage = manage;
        this.transaction= new Stack<KeyValuePair<Member, Book>>();
    }

    public void BorrowBook(Book book, Member member)
    {
        if (!CheckAvailability(book.BookName))
        {
            throw new AlreadyBorrowed($"All copies have been borrowed: {book.BookName}");
        }
        BorrowingHistory history = new BorrowingHistory(member);
        Node<BorrowingHistory> newNode = new Node<BorrowingHistory>(history);
        if (book.BorrowingHistory == null)
        { 
            book.BorrowingHistory = newNode;
        }
        else
        {
            Node<BorrowingHistory> current = book.BorrowingHistory;
            while (current.Next != null)
            {
                current = current.Next;
            }
            current.Next = newNode;
            newNode.Prev = current;
        }
        book.BookStatus = Status.Borrowed;
        transaction.Push(new KeyValuePair<Member, Book>(member, book));
    }

    public bool CheckAvailability(string name)
    {
        return Manage.Catalog.Values.Any(book => book.BookName == name && book.BookStatus == Status.notBorrowed);
    }

    public void ReserveBook(Book book, Member member)
    {
        if (book.BookStatus == Status.Borrowed)
        {
            book.reservation.Enqueue(member);
            return;
        }
        throw new CanGetBook("Book is currently available; reservation is not required.");
    }

    public void RevokeTransaction()
    {
        if (transaction.Count == 0)
            return;

        var transactionData = transaction.Pop();

        Book book = transactionData.Value;

        book.BookStatus = Status.notBorrowed;

        Console.WriteLine($"{transactionData.Key.Name} has revoked the transaction for {book.BookName}");
    }
    
}
