using MiniLibrary.Web.Domain.BookAggregate;
using MiniLibrary.Web.Domain.MemberAggregate;
using Vogen;

namespace MiniLibrary.Web.Infrastructure.Data.Config;

[EfCoreConverter<BookId>]
[EfCoreConverter<BookCopyId>]
[EfCoreConverter<MemberId>]
internal partial class VogenEfCoreConverters;
