using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using MiniLibrary.Web.Domain.BookAggregate;
using MiniLibrary.Web.Extensions;

namespace MiniLibrary.Web.BookFeatures.Delete;

public sealed class DeleteBookRequest
{
  public const string Route = "/books/{BookId}";
  public Guid BookId { get; init; }
}

public class DeleteEndpoint(IMediator mediator) : Endpoint<DeleteBookRequest, Results<NoContent, NotFound, ProblemHttpResult>>
{
  public override void Configure()
  {
    Delete(DeleteBookRequest.Route);
    AllowAnonymous();

    Summary(s =>
    {
      s.Summary = "Delete a book";
      s.Description = "Deletes a book and all its copies by book ID.";
      s.ExampleRequest = new DeleteBookRequest
      {
        BookId = Guid.Parse("12345678-1234-1234-1234-123456789012")
      };

      s.Responses[204] = "Book deleted successfully";
      s.Responses[404] = "Book not found";
    });

    Tags("Books");
  }

  public override async Task<Results<NoContent, NotFound, ProblemHttpResult>>
  ExecuteAsync(DeleteBookRequest request, CancellationToken ct)
  {
    var bookId = BookId.From(request.BookId);
    var command = new DeleteBookCommand(bookId);
    var result = await mediator.Send(command, ct);

    return result.ToDeleteResult();
  }
}
