using MiniLibrary.Web.Domain.BookAggregate;
using MiniLibrary.Web.Domain.BookAggregate.Specifications;

namespace MiniLibrary.Web.BookFeatures.Delete;

public record DeleteBookCommand(BookId BookId) : ICommand<Result>;

public class DeleteBookHandler(IRepository<Book> repository) : ICommandHandler<DeleteBookCommand, Result>
{
  public async ValueTask<Result> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
  {
    var spec = new BookByIdSpec(request.BookId);
    var book = await repository.FirstOrDefaultAsync(spec, cancellationToken);

    if (book == null)
    {
      return Result.NotFound("Book not found");
    }

    await repository.DeleteAsync(book, cancellationToken);
    await repository.SaveChangesAsync(cancellationToken);

    return Result.Success();
  }
}
