using MiniLibrary.Web.Domain.BookAggregate;

namespace MiniLibrary.Web.BookFeatures;

public record BookDto(BookId Id, string Title, string Author, string Isbn, IReadOnlyList<BookCopyDto> Copies);

public record BookCopyDto(BookCopyId Id, string SerialNumber, bool IsAvailable);