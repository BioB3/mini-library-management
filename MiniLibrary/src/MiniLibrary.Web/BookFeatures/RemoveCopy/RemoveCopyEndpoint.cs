using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using MiniLibrary.Web.Domain.BookAggregate;
using MiniLibrary.Web.Extensions;

namespace MiniLibrary.Web.BookFeatures.RemoveCopy;

public sealed class RemoveBookCopyRequest
{
  public const string Route = "/books/{BookId}/copies/{BookCopyId}";
  public Guid BookId { get; init; }
  public Guid BookCopyId { get; init; }
}

public class RemoveCopyEndpoint(IMediator mediator) : Endpoint<RemoveBookCopyRequest, Results<Ok<BookResponse>, NotFound, ProblemHttpResult>, RemoveCopyMapper>
{
  public override void Configure()
  {
    Delete(RemoveBookCopyRequest.Route);
    AllowAnonymous();

    Summary(s =>
    {
      s.Summary = "Remove a copy from a book";
      s.Description = "Removes a physical copy of a book from the catalog.";
      s.ExampleRequest = new RemoveBookCopyRequest
      {
        BookId = Guid.Parse("12345678-1234-1234-1234-123456789012"),
        BookCopyId = Guid.Parse("12345678-1234-4321-4321-123456789012")
      };
      s.ResponseExamples[200] = new BookResponse(
        Guid.Parse("12345678-1234-1234-1234-123456789012"),
        "Example Book",
        "Example Author",
        "978-3-16-148410-0",
        new List<BookCopyResponse>()
      );

      s.Responses[200] = "Book copy removed successfully";
      s.Responses[404] = "Book or book copy not found";
    });

    Tags("Books");
  }

  public override async Task<Results<Ok<BookResponse>, NotFound, ProblemHttpResult>>
  ExecuteAsync(RemoveBookCopyRequest request, CancellationToken ct)
  {
    var bookId = BookId.From(request.BookId);
    var command = new RemoveBookCopyCommand(bookId, request.BookCopyId);
    var result = await mediator.Send(command, ct);

    return result.Status switch
    {
      ResultStatus.Ok => TypedResults.Ok(Map.FromEntity(result.Value)),
      ResultStatus.NotFound => TypedResults.NotFound(),
      _ => TypedResults.Problem(
        title: "Remove copy failed",
        detail: string.Join("; ", result.Errors),
        statusCode: StatusCodes.Status400BadRequest)
    };
  }
}

public sealed class RemoveCopyMapper : Mapper<RemoveBookCopyRequest, BookResponse, BookDto>
{
  public override BookResponse FromEntity(BookDto e)
  {
    var copies = e.Copies.Select(i => new BookCopyResponse(
      i.Id.Value,
      i.SerialNumber,
      i.IsAvailable
    )).ToList();

    return new BookResponse(e.Id.Value, e.Title, e.Author, e.Isbn, copies);
  }
}
