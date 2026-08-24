
This project is a Library Management System developed in C#. The main purpose of the project is to manage books, members, borrowing history, reservations, transactions and book lookup using different data structures.

I have divided the project into four directories for a proper folder structure and separation of responsibilities:

* model
* repository
* service
* exceptions

I have also created a separate NUnit test project for testing the functionality.

Project Structure

Model

The model directory contains four POCO classes:

* Book
* BorrowingHistory
* Member
* Node

Book

Book is the main class of the system. It contains the book name, author, ISBN, borrowing history, book status and reservation queue.

The borrowing history is maintained using:
Node<BorrowingHistory>

This means each book can maintain its own borrowing history.
BorrowingHistory

BorrowingHistory represents a borrowing record. It stores the member who borrowed the book.
I created a separate BorrowingHistory class instead of directly storing a member in the linked list because it represents a particular borrowing record.

Member

Member represents a library member. It contains the member ID, name, address and phone number.
I use a static IdCounter to generate member IDs automatically when a new member is created.

Node

Node<T> is my generic node class.

I made the node generic so that it can be reused with different types instead of creating a separate node class for every type.

For example:

Node<BorrowingHistory>

is used for borrowing history but the same generic node can also be used with other types.

The node contains both Next and Prev references, so it can be used to maintain a doubly linked list. This allows me to traverse the borrowing history in both forward and backward directions.

Repository

The repository directory contains the repository classes used for storing and accessing data.

HashMapRepo

I created a generic HashMapRepo<TKey, TEntity> using C#’s Dictionary.

For books, the HashMap is used with:

ISBN -> Book

This allows the system to identify a book using its ISBN.

For members, the member ID can be used as the key.

The repository is generic so the same implementation can be reused for different types of keys and entities.

I also expose the values through IEnumerable<TEntity> so that I can use LINQ when I need to search through the stored entities.

Service

The service directory contains the business logic of the library system.

ManagementService

ManagementService is responsible for managing the main collections used by the library.

It maintains the book catalog and members using the repository.

BorrowingService

BorrowingService handles borrowing and reservations.

Before borrowing a book, I check whether the book is available.

I use LINQ Any() to check if a book with the given name exists and has BookStatus set to notBorrowed.

When a book is borrowed, I create a new BorrowingHistory object and put it inside a new Node<BorrowingHistory>.

If the book does not have any previous history, the new node becomes the first node. Otherwise, I traverse to the end of the linked list and add the new node there.

After borrowing, the book status is changed to Borrowed.

I also use a Stack for transactions so that the latest transaction can be revoked first.

ReturningService

ReturningService handles returning books.

When a book is returned, I first check whether the book is currently borrowed.

If there is no reservation for the book, its status is changed to notBorrowed.

If there are members waiting for the book, I use the reservation queue and remove the first member using Dequeue().

This follows the FIFO principle, so the member who reserved the book first gets it first.

Data Structures Used

Doubly Linked List

I use a doubly linked list for maintaining borrowing history.

Each node contains a Prev and Next reference, which allows the history to be traversed in both directions.

Adding a new history record currently takes O(n) because I traverse to the last node before inserting it.

Forward and backward traversal both take O(n) time.

The borrowing history requires O(n) space for n history records.

Queue

I use a queue for the reservation waitlist.

Reservations follow FIFO order.

Adding a reservation using Enqueue() takes O(1) time, and removing the first reservation using Dequeue() also takes O(1) time.

The queue requires O(n) space for n reservations.

Stack

I use a stack for transaction undo.

The reason for using a stack is that undo should work on the latest transaction first, which follows the LIFO principle.

Push() and Pop() both take O(1) time.

HashMap

I use a HashMap for book lookup using ISBN.

The average lookup complexity is O(1).

The worst-case lookup can be O(n).

Complexity Analysis

Checkout

Checking book availability using LINQ over the catalog takes O(n) in the worst case.

Adding the borrowing history currently takes O(n) because I traverse to the end of the linked list.

Therefore, the checkout operation is O(n) overall for the current implementation.

The additional space used for the new borrowing record and transaction is O(1), while the complete borrowing history requires O(n) space.

Return

Checking the book status takes O(1).

If there is no reservation, returning the book takes O(1).

If there is a reservation, Dequeue() takes O(1).

Adding the new borrowing history record can take O(n) because the linked list is traversed to the end.

Therefore, the return operation can take O(n) in the current implementation.

Reservation

Adding a member to the reservation queue takes O(1).

Undo

Removing the latest transaction from the stack takes O(1).

Book Lookup

HashMap lookup by ISBN has an average time complexity of O(1).

Borrowing History

Forward traversal takes O(n).

Backward traversal takes O(n).

The linked list requires O(n) space for n borrowing records.

Testing

I have used NUnit for testing the system.

The test cases cover more than the required 10 tests.

The tests cover:

* Adding books
* Duplicate ISBN
* Book availability
* Invalid book lookup
* Borrowing a book
* Borrowing an already borrowed book
* Creating borrowing history
* Adding reservations
* Multiple reservations
* FIFO reservation behavior
* Returning a book
* Returning an unborrowed book
* Returning a book with reservations
* Maintaining multiple borrowing-history records
* Forward traversal
* Backward traversal
* Undoing a transaction
* Undo when there are no transactions

I have also included two integrated workflows.

Integrated Workflow 1

The first workflow tests:

1. Check book availability
2. Borrow the book
3. Verify that the book is unavailable
4. Return the book
5. Verify that the book becomes available again

Integrated Workflow 2

The second workflow tests:

1. One member borrows a book
2. Two other members reserve the book
3. The original member returns the book
4. The first member in the reservation queue gets the book
5. The second reservation remains in the queue

This verifies that borrowing, returning, reservations, queues and borrowing history work together.

Edge Cases

The implementation and tests cover the following edge cases:

* Book is already borrowed
* Book does not exist
* Trying to reserve an available book
* Multiple reservations for the same book
* Empty reservation queue
* Empty transaction stack
* Duplicate ISBN
* Returning a book that is not borrowed



The main approach I used was to keep the model classes simple and put the actual operations inside the service classes.

I used a generic node so that the linked-list implementation can be reused. The doubly linked list is used for borrowing history, the queue is used for reservations, the stack is used for transaction undo, and the HashMap is used for ISBN-based book lookup.

Each data structure is used according to the operation it is suitable for, while the service layer handles the actual library operations.
