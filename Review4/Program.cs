using Review4.models;
using Review4.service;


using Review4.models;
using Review4.service;

namespace Review4;

public class Program
{
    public static void Main(string[] args)
    {
        ManagementService management = new ManagementService();
        BorrowingService borrowing = new BorrowingService(management);
        ReturningService returning = new ReturningService(management);
        Book book1 = new Book("Clean Code", "Robert frost", 101);
        Book book2 = new Book("The Pragmatic Programmer", "spiderman", 102);
        
        Member member1 = new Member("jerry", "Punjab", 987654321);
        Member member2 = new Member("tom", "Delhi", 987654322);
        management.Catalog.AddToHashMap(book1.isbn, book1);
        management.Catalog.AddToHashMap(book2.isbn, book2);
        Console.WriteLine($"Clean Code available: {borrowing.CheckAvailability("Clean Code")}");
        borrowing.BorrowBook(book1, member1);
        Console.WriteLine($"Book status: {book1.BookStatus}");
        Console.WriteLine($"Clean Code available: " + $"{borrowing.CheckAvailability("Clean Code")}");
        borrowing.ReserveBook(book1, member2);
        Console.WriteLine($"Waiting list size: {book1.reservation.Count}");
        returning.ReturnBook(book1, member1);
        Console.WriteLine($"Book status: {book1.BookStatus}");
        Console.WriteLine($"Waiting list size: {book1.reservation.Count}");
        Node<BorrowingHistory> current = book1.BorrowingHistory;
        while (current != null)
        {
            Console.WriteLine($"Borrowed by: {current.Data.Member.Name}");
            current = current.Next;
        }
    }
}