using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using MiniLibrary.Web.Domain.BookAggregate;
using MiniLibrary.Web.BookFeatures;

namespace MiniLibrary.Web.BookFeatures.Create;

public sealed class CreateBookRequest
{ 
  public string Title { get; init; } = string.Empty;
  public string Author { get; init; } = string.Empty;
  public string Isbn {get; init;} = string.Empty;
}

public class CreateEndpoint(IRepository<Book> repository) : 
  Endpoint<CreateBookRequest, 
          Results<Created<BookResponse>, ValidationProblem, ProblemHttpResult>>
{
  private readonly IRepository<Book> _repository = repository;

  public override void Configure()
  {
    Post("/Books");
    AllowAnonymous();

  Summary(s =>
    {
        s.Summary = "Add a new book to the catalog";
        s.Description = "Adds a new book with the specified title, author, and ISBN.";
        s.ExampleRequest = new CreateBookRequest { Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", Isbn = "9780743273565" };
        s.ResponseExamples[201] = new BookResponse(Guid.NewGuid(), "The Great Gatsby", "F. Scott Fitzgerald", "9780743273565", new List<BookCopyResponse>());

        s.Responses[201] = "Book added successfully";
        s.Responses[400] = "Invalid request data";
    });

    Tags("Books");
  }

  public override async Task<Results<Created<BookResponse>, ValidationProblem, ProblemHttpResult>> 
    ExecuteAsync(CreateBookRequest request, CancellationToken cancellationToken)
  {
    var book = Book.Create(request.Title, request.Author, request.Isbn);

    await _repository.AddAsync(book, cancellationToken);
    await _repository.SaveChangesAsync(cancellationToken);

    var response = new BookResponse(book.Id.Value, book.Title, book.Author, book.Isbn, new List<BookCopyResponse>());
    return TypedResults.Created($"/Books/{book.Id.Value}", response);
  }
}

public sealed class CreateBookValidator : Validator<CreateBookRequest>
{
  public CreateBookValidator()
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
