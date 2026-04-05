using MiniLibrary.Web.Domain.ProductAggregate;

namespace MiniLibrary.Web.ProductFeatures;
public record ProductDto(ProductId Id, string Name, decimal UnitPrice);
