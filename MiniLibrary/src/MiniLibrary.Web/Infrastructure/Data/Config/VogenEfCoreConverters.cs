using MiniLibrary.Web.Domain.CartAggregate;
using MiniLibrary.Web.Domain.GuestUserAggregate;
using MiniLibrary.Web.Domain.OrderAggregate;
using MiniLibrary.Web.Domain.ProductAggregate;
using Vogen;

namespace MiniLibrary.Web.Infrastructure.Data.Config;

[EfCoreConverter<ProductId>]
[EfCoreConverter<CartId>]
[EfCoreConverter<CartItemId>]
[EfCoreConverter<GuestUserId>]
[EfCoreConverter<OrderId>]
[EfCoreConverter<OrderItemId>]
[EfCoreConverter<Quantity>]
[EfCoreConverter<Price>]
internal partial class VogenEfCoreConverters;
