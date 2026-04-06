using Vogen;

namespace MiniLibrary.Web.Domain.BookAggregate;

[ValueObject<Guid>]
public readonly partial struct BookCopyId
{
  private static Validation Validate(Guid value)
      => value != Guid.Empty ? Validation.Ok : Validation.Invalid("BookCopyId cannot be empty.");
}
