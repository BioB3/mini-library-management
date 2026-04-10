namespace MiniLibrary.Web.Domain.BookAggregate.Specifications;

public class BookByIdSpec : Specification<Book>
{
  public BookByIdSpec(BookId bookId) =>
    Query
      .Include(b => b.Copies)
      .Where(book => book.Id == bookId);
}