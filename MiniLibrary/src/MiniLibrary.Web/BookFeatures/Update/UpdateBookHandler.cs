using MiniLibrary.Web.Domain.BookAggregate;
using MiniLibrary.Web.Domain.BookAggregate.Specifications;

namespace MiniLibrary.Web.BookFeatures.Update;

public record UpdateBookCommand(BookId BookId, string Title, string Author, string Isbn) : ICommand<Result<BookDto>>;

public class UpdateBookHandler(IRepository<Book> repository) : ICommandHandler<UpdateBookCommand, Result<BookDto>>
{
  public async ValueTask<Result<BookDto>> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
  {
    var spec = new BookByIdSpec(request.BookId);
    var book = await repository.FirstOrDefaultAsync(spec, cancellationToken);

    if (book == null)
    {
      return Result.NotFound("Book not found");
    }

    book.Update(request.Title, request.Author, request.Isbn);
    await repository.SaveChangesAsync(cancellationToken);

    var copies = book.Copies.Select(i => new BookCopyDto(
      i.Id,
      i.SerialNumber,
      i.IsAvailable
    )).ToList();

    return new BookDto(book.Id, book.Title, book.Author, book.Isbn, copies);
  }
}
