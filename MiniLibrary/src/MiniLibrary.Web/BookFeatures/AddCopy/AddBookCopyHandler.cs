using MiniLibrary.Web.Domain.BookAggregate;
using MiniLibrary.Web.Domain.BookAggregate.Specifications;

namespace MiniLibrary.Web.BookFeatures.AddCopy;

public record AddBookCopyCommand(BookId BookId, string SerialNumber) : ICommand<Result<BookDto>>;

public class AddBookCopyHandler(IRepository<Book> repository) : ICommandHandler<AddBookCopyCommand, Result<BookDto>>
{
  public async ValueTask<Result<BookDto>> Handle(AddBookCopyCommand request, CancellationToken cancellationToken)
  {
    var spec = new BookByIdSpec(request.BookId);
    var book = await repository.FirstOrDefaultAsync(spec, cancellationToken);

    if (book == null)
    {
      return Result.NotFound("Book not found");
    }

    book.AddCopy(request.SerialNumber);
    await repository.SaveChangesAsync(cancellationToken);

    var copies = book.Copies.Select(i => new BookCopyDto(
      i.Id,
      i.SerialNumber,
      i.IsAvailable
    )).ToList();

    return new BookDto(book.Id, book.Title, book.Author, book.Isbn, copies);
  }
}
