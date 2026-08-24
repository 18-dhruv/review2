using NUnit.Framework;
using Review4.Exceptions;
using Review4.models;
using Review4.service;

namespace Review4.Tests;

[TestFixture]
public class LibraryTests
{
    private ManagementService management;
    private BorrowingService borrowing;
    private ReturningService returning;

    private Book book1;
    private Book book2;

    private Member member1;
    private Member member2;
    private Member member3;

    [SetUp]
    public void Setup()
    {
        management = new ManagementService();
        borrowing = new BorrowingService(management);
        returning = new ReturningService(management);

        book1 = new Book("Clean Code", "Robert Martin", 1001);
        book2 = new Book("The Pragmatic Programmer", "Andrew Hunt", 1002);

        member1 = new Member("Dhruv", "Punjab", 111111111);
        member2 = new Member("Alice", "Delhi", 222222222);
        member3 = new Member("Bob", "Mumbai", 333333333);

        management.Catalog.AddToHashMap(book1.isbn, book1);
        management.Catalog.AddToHashMap(book2.isbn, book2);
    }

    [Test]
    public void AddBook_ShouldAddBook()
    {
        Assert.That(management.Catalog.Values, Does.Contain(book1));
    }

    [Test]
    public void AddBook_WithSameISBN_ShouldThrowException()
    {
        Book duplicate = new Book(
            "Another Book",
            "Another Author",
            1001
        );

        Assert.Throws<AlreadyExistInHashMap>(() =>
            management.Catalog.AddToHashMap(duplicate.isbn, duplicate));
    }

    [Test]
    public void CheckAvailability_WhenBookExists_ShouldReturnTrue()
    {
        Assert.That(
            borrowing.CheckAvailability("Clean Code"),
            Is.True
        );
    }

    [Test]
    public void CheckAvailability_WhenBookDoesNotExist_ShouldReturnFalse()
    {
        Assert.That(
            borrowing.CheckAvailability("Unknown Book"),
            Is.False
        );
    }

    [Test]
    public void BorrowBook_ShouldChangeBookStatus()
    {
        borrowing.BorrowBook(book1, member1);

        Assert.That(
            book1.BookStatus,
            Is.EqualTo(Status.Borrowed)
        );
    }

    [Test]
    public void BorrowBook_ShouldCreateHistory()
    {
        borrowing.BorrowBook(book1, member1);

        Assert.That(book1.BorrowingHistory, Is.Not.Null);
        Assert.That(
            book1.BorrowingHistory.Data.Member,
            Is.EqualTo(member1)
        );
    }

    [Test]
    public void BorrowBook_WhenAlreadyBorrowed_ShouldThrowException()
    {
        borrowing.BorrowBook(book1, member1);

        Assert.Throws<AlreadyBorrowed>(() =>
            borrowing.BorrowBook(book1, member2));
    }

    [Test]
    public void ReserveBook_ShouldAddMemberToQueue()
    {
        borrowing.BorrowBook(book1, member1);
        borrowing.ReserveBook(book1, member2);

        Assert.That(book1.reservation.Count, Is.EqualTo(1));
        Assert.That(book1.reservation.Peek(), Is.EqualTo(member2));
    }

    [Test]
    public void Reservations_ShouldFollowFIFO()
    {
        borrowing.BorrowBook(book1, member1);

        borrowing.ReserveBook(book1, member2);
        borrowing.ReserveBook(book1, member3);

        Assert.That(
            book1.reservation.Dequeue(),
            Is.EqualTo(member2)
        );

        Assert.That(
            book1.reservation.Dequeue(),
            Is.EqualTo(member3)
        );
    }

    [Test]
    public void ReserveBook_WhenBookIsAvailable_ShouldThrowException()
    {
        Assert.Throws<CanGetBook>(() =>
            borrowing.ReserveBook(book1, member2));
    }

    [Test]
    public void ReturnBook_ShouldMakeBookAvailable()
    {
        borrowing.BorrowBook(book1, member1);

        returning.ReturnBook(book1, member1);

        Assert.That(
            book1.BookStatus,
            Is.EqualTo(Status.notBorrowed)
        );
    }

    [Test]
    public void ReturnBook_WhenBookIsNotBorrowed_ShouldThrowException()
    {
        Assert.Throws<AlreadyBorrowed>(() =>
            returning.ReturnBook(book1, member1));
    }

    [Test]
    public void ReturnBook_ShouldGiveBookToFirstMemberInQueue()
    {
        borrowing.BorrowBook(book1, member1);
        borrowing.ReserveBook(book1, member2);

        returning.ReturnBook(book1, member1);

        Assert.That(
            book1.BookStatus,
            Is.EqualTo(Status.Borrowed)
        );

        Assert.That(
            book1.reservation.Count,
            Is.EqualTo(0)
        );
    }

    [Test]
    public void ReturnBook_WithMultipleReservations_ShouldKeepRemainingMembers()
    {
        borrowing.BorrowBook(book1, member1);

        borrowing.ReserveBook(book1, member2);
        borrowing.ReserveBook(book1, member3);

        returning.ReturnBook(book1, member1);

        Assert.That(book1.reservation.Count, Is.EqualTo(1));
        Assert.That(book1.reservation.Peek(), Is.EqualTo(member3));
    }

    [Test]
    public void BorrowingHistory_ShouldKeepPreviousBorrowers()
    {
        borrowing.BorrowBook(book1, member1);
        returning.ReturnBook(book1, member1);

        borrowing.BorrowBook(book1, member2);

        var first = book1.BorrowingHistory;

        Assert.That(first.Data.Member, Is.EqualTo(member1));
        Assert.That(first.Next.Data.Member, Is.EqualTo(member2));
    }

    [Test]
    public void BorrowingHistory_ShouldAllowBackwardTraversal()
    {
        borrowing.BorrowBook(book1, member1);
        returning.ReturnBook(book1, member1);

        borrowing.BorrowBook(book1, member2);

        var current = book1.BorrowingHistory;

        while (current.Next != null)
        {
            current = current.Next;
        }

        Assert.That(current.Data.Member, Is.EqualTo(member2));
        Assert.That(current.Prev.Data.Member, Is.EqualTo(member1));
    }

    [Test]
    public void RevokeTransaction_ShouldMakeBookAvailable()
    {
        borrowing.BorrowBook(book1, member1);

        borrowing.RevokeTransaction();

        Assert.That(
            book1.BookStatus,
            Is.EqualTo(Status.notBorrowed)
        );
    }

    [Test]
    public void RevokeTransaction_WhenStackIsEmpty_ShouldNotThrow()
    {
        Assert.DoesNotThrow(() =>
            borrowing.RevokeTransaction());
    }

    [Test]
    public void BorrowAndReturn_ShouldWorkTogether()
    {
        Assert.That(
            borrowing.CheckAvailability("Clean Code"),
            Is.True
        );

        borrowing.BorrowBook(book1, member1);

        Assert.That(
            borrowing.CheckAvailability("Clean Code"),
            Is.False
        );

        returning.ReturnBook(book1, member1);

        Assert.That(
            borrowing.CheckAvailability("Clean Code"),
            Is.True
        );
    }

    [Test]
    public void BorrowReserveAndReturn_ShouldFollowCorrectFlow()
    {
        borrowing.BorrowBook(book1, member1);

        borrowing.ReserveBook(book1, member2);
        borrowing.ReserveBook(book1, member3);

        Assert.That(book1.reservation.Count, Is.EqualTo(2));

        returning.ReturnBook(book1, member1);
        
        Assert.That(
            book1.BookStatus,
            Is.EqualTo(Status.Borrowed)
        );
        
        Assert.That(book1.reservation.Count, Is.EqualTo(1));
        Assert.That(
            book1.reservation.Peek(),
            Is.EqualTo(member3)
        );
    }
}