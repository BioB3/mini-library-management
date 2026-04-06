namespace MiniLibrary.Web.Domain.BookAggregate;

public class BookCopy : EntityBase<BookCopy, BookCopyId>
{
  public string SerialNumber { get; private set; }
  public bool IsAvailable { get; private set; } = true;

  public BookCopy(BookCopyId id, string serialNumber)
  {
    Id = id;
    SerialNumber = serialNumber;
  }
}