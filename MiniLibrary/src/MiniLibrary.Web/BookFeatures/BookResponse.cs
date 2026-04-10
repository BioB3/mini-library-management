namespace MiniLibrary.Web.BookFeatures;

public record BookResponse(Guid BookId, string Title, string Author, string Isbn, IReadOnlyList<BookCopyResponse> Copies);

public record BookCopyResponse(Guid BookCopyId, string SerialNumber, bool IsAvailable);