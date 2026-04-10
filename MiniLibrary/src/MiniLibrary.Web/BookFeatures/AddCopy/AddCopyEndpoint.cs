using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using MiniLibrary.Web.Domain.BookAggregate;
using MiniLibrary.Web.Extensions;

namespace MiniLibrary.Web.BookFeatures.AddCopy;

public sealed class AddBookCopyRequest
{
  public const string Route = "/books/{BookId}/copies";
  public Guid BookId { get; init; }
  public string SerialNumber { get; init; } = string.Empty;
}

public class AddCopyEndpoint(IMediator mediator) : Endpoint<AddBookCopyRequest, Results<Created<BookResponse>, NotFound, ProblemHttpResult>, AddCopyMapper>
{
  public override void Configure()
  {
    Post(AddBookCopyRequest.Route);
    AllowAnonymous();

    Summary(s =>
    {
      s.Summary = "Add a copy to a book";
      s.Description = "Adds a new physical copy of an existing book to the catalog.";
      s.ExampleRequest = new AddBookCopyRequest
      {
        BookId = Guid.Parse("12345678-1234-1234-1234-123456789012"),
        SerialNumber = "BOOK-001"
      };
      s.ResponseExamples[201] = new BookResponse(
        Guid.Parse("12345678-1234-1234-1234-123456789012"),
        "Example Book",
        "Example Author",
        "978-3-16-148410-0",
        new List<BookCopyResponse>
        {
          new(Guid.Parse("12345678-1234-4321-4321-123456789012"), "BOOK-001", true)
        }
      );

      s.Responses[201] = "Book copy added successfully";
      s.Responses[404] = "Book not found";
    });

    Tags("Books");
  }

  public override async Task<Results<Created<BookResponse>, NotFound, ProblemHttpResult>>
  ExecuteAsync(AddBookCopyRequest request, CancellationToken ct)
  {
    var bookId = BookId.From(request.BookId);
    var command = new AddBookCopyCommand(bookId, request.SerialNumber);
    var result = await mediator.Send(command, ct);

    return result.Status switch
    {
      ResultStatus.Ok => TypedResults.Created($"/books/{request.BookId}/copies", Map.FromEntity(result.Value)),
      ResultStatus.NotFound => TypedResults.NotFound(),
      _ => TypedResults.Problem(
        title: "Add copy failed",
        detail: string.Join("; ", result.Errors),
        statusCode: StatusCodes.Status400BadRequest)
    };
  }
}

public sealed class AddCopyMapper : Mapper<AddBookCopyRequest, BookResponse, BookDto>
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

public sealed class AddBookCopyValidator : Validator<AddBookCopyRequest>
{
  public AddBookCopyValidator()
  {
    RuleFor(x => x.SerialNumber)
      .NotEmpty()
      .WithMessage("Serial number is required")
      .MaximumLength(100)
      .WithMessage("Serial number must not exceed 100 characters");
  }
}
