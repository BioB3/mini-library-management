using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using MiniLibrary.Web.Extensions;

namespace MiniLibrary.Web.BookFeatures.List;

public class ListEndpoint(IMediator mediator) : EndpointWithoutRequest<Ok<List<BookResponse>>>
{
  public override void Configure()
  {
    Get("/books");
    AllowAnonymous();

    Summary(s =>
    {
      s.Summary = "List all books";
      s.Description = "Retrieves a list of all books in the catalog with their copies.";
      s.ResponseExamples[200] = new List<BookResponse>
      {
        new(
          Guid.Parse("12345678-1234-1234-1234-123456789012"),
          "Why you should read this",
          "John",
          "978-3-16-148410-0",
          new List<BookCopyResponse>
          {
            new(Guid.Parse("12345678-1234-4321-4321-123456789012"), "001", true)
          }
        )
      };

      s.Responses[200] = "Books retrieved successfully";
    });

    Tags("Books");
  }

  public override async Task<Ok<List<BookResponse>>>
  ExecuteAsync(CancellationToken ct)
  {
    var query = new ListBooksQuery();
    var result = await mediator.Send(query, ct);

    var response = result.Value.Select(dto => new BookResponse(
      dto.Id.Value,
      dto.Title,
      dto.Author,
      dto.Isbn,
      dto.Copies.Select(i => new BookCopyResponse(i.Id.Value, i.SerialNumber, i.IsAvailable)).ToList()
    )).ToList();

    return TypedResults.Ok(response);
  }
}
