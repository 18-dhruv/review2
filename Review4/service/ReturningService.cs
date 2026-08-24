using Review4.Exceptions;
using Review4.models;

namespace Review4.service;

public class ReturningService
{
    public readonly ManagementService Manage;

    public ReturningService(ManagementService manage)
    {
        Manage = manage;
    }

    public void ReturnBook(Book book, Member member)
    {
        if (book.BookStatus == Status.notBorrowed)
        {
            throw new AlreadyBorrowed($"Book is not currently borrowed: {book.BookName}");
        }

        book.BookStatus = Status.notBorrowed;

        Console.WriteLine($"{member.Name} returned {book.BookName}");

        if (book.reservation.Count > 0)
        {
            Member nextMember = book.reservation.Dequeue();

            book.BookStatus = Status.Borrowed;

            BorrowingHistory history = new BorrowingHistory(nextMember);

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

            Console.WriteLine($"{book.BookName} is now borrowed by {nextMember.Name}");
        }
    }
}