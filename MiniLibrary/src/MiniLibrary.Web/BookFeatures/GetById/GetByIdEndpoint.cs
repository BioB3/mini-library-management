using MiniLibrary.Web.Domain.BookAggregate;
using MiniLibrary.Web.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using FastEndpoints;

namespace MiniLibrary.Web.BookFeatures.GetById;

public sealed class GetBookRequest
{
  public const string Route = "/books/{BookId}";
  public Guid BookId { get; init; }
}

public class GetByIdEndpoint(IMediator mediator) : Endpoint<GetBookRequest, Results<Ok<BookResponse>, NotFound, ProblemHttpResult>, GetBookMapper>
{
  public override void Configure()
  {
    Get(GetBookRequest.Route);
    AllowAnonymous();

    Summary(s =>
    {
      s.Summary = "Get Book by ID";
      s.Description = "Retrieves a book and all its copies by book ID.";
      s.ExampleRequest = new GetBookRequest
      {
        BookId = Guid.Parse("12345678-1234-1234-1234-123456789012")
      };
      s.ResponseExamples[200] = new BookResponse(
      Guid.Parse("12345678-1234-1234-1234-123456789012"),
      "Why you should read this",
      "John",
      "978-3-16-148410-0",
      new List<BookCopyResponse>
      {
        new(Guid.Parse("12345678-1234-4321-4321-123456789012"), "123", true)
      });

      s.Responses[200] = "Book retrieved successfully";
      s.Responses[404] = "Book not found";
    });

    Tags("Books");

    Description(builder => builder
    .Produces<BookResponse>(200, "application/json").ProducesProblem(404));
  }

  public override async Task<Results<Ok<BookResponse>, NotFound, ProblemHttpResult>>
  ExecuteAsync(GetBookRequest request, CancellationToken ct)
  {
    var bookId = BookId.From(request.BookId);
    var query = new GetBookQuery(bookId);
    var result = await mediator.Send(query, ct);

    return result.ToGetByIdResult(Map.FromEntity);
  }
}

public sealed class GetBookMapper : Mapper<GetBookRequest, BookResponse, BookDto>
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