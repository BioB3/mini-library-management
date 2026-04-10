using MiniLibrary.Web.Domain.BookAggregate;
using MiniLibrary.Web.Domain.BookAggregate.Specifications;

namespace MiniLibrary.Web.BookFeatures.List;

public record ListBooksQuery : IQuery<Result<List<BookDto>>>;

public class ListBooksHandler(IReadRepository<Book> repository) : IQueryHandler<ListBooksQuery, Result<List<BookDto>>>
{
  public async ValueTask<Result<List<BookDto>>> Handle(ListBooksQuery request, CancellationToken cancellationToken)
  {
    var spec = new BooksSpec();
    var books = await repository.ListAsync(spec, cancellationToken);

    var bookDtos = books.Select(book => new BookDto(
      book.Id,
      book.Title,
      book.Author,
      book.Isbn,
      book.Copies.Select(i => new BookCopyDto(
        i.Id,
        i.SerialNumber,
        i.IsAvailable
      )).ToList()
    )).ToList();

    return bookDtos;
  }
}
