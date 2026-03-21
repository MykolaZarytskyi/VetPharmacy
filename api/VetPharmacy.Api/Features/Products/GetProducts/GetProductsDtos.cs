namespace VetPharmacy.Api.Features.Products.GetProducts;

public record GetProductsDto(
    int PageNumber = 1,
    int PageSize = 5,
    string? Name = null);

public record ProductsPageDto(int TotalPages, IEnumerable<ProductSummaryDto> Data);

public record ProductSummaryDto(
    Guid Id,
    string Name,
    decimal Price,
    string LastModifiedBy,
    string ImageUri,
    string? Description = null);
