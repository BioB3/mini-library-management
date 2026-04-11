using MiniLibrary.Web.Domain.BookAggregate;
using MiniLibrary.Web.Domain.BookAggregate.Specifications;

namespace MiniLibrary.Web.BookFeatures.RemoveCopy;

public record RemoveBookCopyCommand(BookId BookId, Guid BookCopyId) : ICommand<Result<BookDto>>;

public class RemoveBookCopyHandler(IRepository<Book> repository) : ICommandHandler<RemoveBookCopyCommand, Result<BookDto>>
{
  public async ValueTask<Result<BookDto>> Handle(RemoveBookCopyCommand request, CancellationToken cancellationToken)
  {
    var spec = new BookByIdSpec(request.BookId);
    var book = await repository.FirstOrDefaultAsync(spec, cancellationToken);

    if (book == null)
    {
      return Result.NotFound("Book not found");
    }

    var copyId = BookCopyId.From(request.BookCopyId);
    var removed = book.RemoveCopy(copyId);

    if (!removed)
    {
      return Result.NotFound("Book copy not found");
    }

    await repository.SaveChangesAsync(cancellationToken);

    var copies = book.Copies.Select(i => new BookCopyDto(
      i.Id,
      i.SerialNumber,
      i.IsAvailable
    )).ToList();

    return new BookDto(book.Id, book.Title, book.Author, book.Isbn, copies);
  }
}
