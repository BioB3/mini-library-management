namespace MiniLibrary.Web.Domain.BookAggregate.Specifications;

public class BooksSpec : Specification<Book>
{
  public BooksSpec() =>
    Query.Include(b => b.Copies);
}
