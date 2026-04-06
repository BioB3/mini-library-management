using MiniLibrary.Web.Domain.BookAggregate;
using Vogen;

namespace MiniLibrary.Web.Infrastructure.Data.Config;

[EfCoreConverter<BookId>]
[EfCoreConverter<BookCopyId>]
internal partial class VogenEfCoreConverters;
