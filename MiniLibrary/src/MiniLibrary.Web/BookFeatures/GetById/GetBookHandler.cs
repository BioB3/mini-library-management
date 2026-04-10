using MiniLibrary.Web.Domain.BookAggregate;
using MiniLibrary.Web.Domain.BookAggregate.Specifications;

namespace MiniLibrary.Web.BookFeatures.GetById;

public record GetBookQuery(BookId BookId) : IQuery<Result<BookDto>>;
public class GetBookHandler(IReadRepository<Domain.BookAggregate.Book> repository) : IQueryHandler<GetBookQuery, Result<BookDto>>
{
  public async ValueTask<Result<BookDto>> Handle(GetBookQuery request, CancellationToken cancellationToken)
  {
    var spec = new BookByIdSpec(request.BookId);
    var book = await repository.FirstOrDefaultAsync(spec, cancellationToken);

    if (book == null)
    {
      return Result.NotFound("Book not found");
    }

    var copies = book.Copies.Select(i => new BookCopyDto(
    i.Id,
    i.SerialNumber,
    i.IsAvailable
    )).ToList();

    return new BookDto(book.Id, book.Title, book.Author, book.Isbn, copies);
  }
}