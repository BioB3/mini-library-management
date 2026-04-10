using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using MiniLibrary.Web.Domain.BookAggregate;
using MiniLibrary.Web.Extensions;

namespace MiniLibrary.Web.BookFeatures.Update;

public sealed class UpdateBookRequest
{
  public const string Route = "/books/{BookId}";
  public Guid BookId { get; init; }
  public string Title { get; init; } = string.Empty;
  public string Author { get; init; } = string.Empty;
  public string Isbn { get; init; } = string.Empty;
}

public class UpdateEndpoint(IMediator mediator) : Endpoint<UpdateBookRequest, Results<Ok<BookResponse>, NotFound, ProblemHttpResult>, UpdateBookMapper>
{
  public override void Configure()
  {
    Put(UpdateBookRequest.Route);
    AllowAnonymous();

    Summary(s =>
    {
      s.Summary = "Update a book";
      s.Description = "Updates an existing book's details by book ID.";
      s.ExampleRequest = new UpdateBookRequest
      {
        BookId = Guid.Parse("12345678-1234-1234-1234-123456789012"),
        Title = "Updated Title",
        Author = "Updated Author",
        Isbn = "978-3-16-148410-0"
      };
      s.ResponseExamples[200] = new BookResponse(
        Guid.Parse("12345678-1234-1234-1234-123456789012"),
        "Updated Title",
        "Updated Author",
        "978-3-16-148410-0",
        new List<BookCopyResponse>()
      );

      s.Responses[200] = "Book updated successfully";
      s.Responses[404] = "Book not found";
    });

    Tags("Books");
  }

  public override async Task<Results<Ok<BookResponse>, NotFound, ProblemHttpResult>>
  ExecuteAsync(UpdateBookRequest request, CancellationToken ct)
  {
    var bookId = BookId.From(request.BookId);
    var command = new UpdateBookCommand(bookId, request.Title, request.Author, request.Isbn);
    var result = await mediator.Send(command, ct);

    return result.ToUpdateResult(Map.FromEntity);
  }
}

public sealed class UpdateBookMapper : Mapper<UpdateBookRequest, BookResponse, BookDto>
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

public sealed class UpdateBookValidator : Validator<UpdateBookRequest>
{
  public UpdateBookValidator()
  {
    RuleFor(x => x.Title)
      .NotEmpty()
      .WithMessage("Book title is required")
      .MaximumLength(500)
      .WithMessage("Book title must not exceed 500 characters");

    RuleFor(x => x.Author)
      .NotEmpty()
      .WithMessage("Author name is required")
      .MaximumLength(200)
      .WithMessage("Author name must not exceed 200 characters");

    RuleFor(x => x.Isbn)
      .NotEmpty()
      .WithMessage("ISBN is required")
      .Matches(@"^(?:ISBN(?:-1[03])?:? )?(?=[0-9X]{10}$|(?=(?:[0-9]+[- ]){3})[- 0-9X]{13}$|97[89][0-9]{10}$|(?=(?:[0-9]+[- ]){4})[- 0-9]{17}$)(?:97[89][- ]?)?[0-9]{1,5}[- ]?[0-9]+[- ]?[0-9]+[- ]?[0-9X]$")
      .WithMessage("ISBN must be in a valid format");
  }
}
