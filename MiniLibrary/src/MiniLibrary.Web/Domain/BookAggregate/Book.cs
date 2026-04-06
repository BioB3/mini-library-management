namespace MiniLibrary.Web.Domain.BookAggregate;

public class Book : EntityBase<Book, BookId>, IAggregateRoot
{
  public string Title { get; private set; }
  public string Author { get; private set; }
  public string Isbn { get; private set; }
  
  private readonly List<BookCopy> _copies = new();
  public IEnumerable<BookCopy> Copies => _copies.AsReadOnly();

  public Book(BookId id, string title, string author, string isbn)
  {
    Id = id;
    Title = title;
    Author = author;
    Isbn = isbn;
  }

  public void AddCopy(string serialNumber)
  {
    var copyId = BookCopyId.From(Guid.NewGuid());
    var newCopy = new BookCopy(copyId, serialNumber);

    _copies.Add(newCopy);
  }
}