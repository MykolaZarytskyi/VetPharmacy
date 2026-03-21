namespace VetPharmacy.Api.Features.Products.GetProduct;

public record ProductDetailsDto(
    Guid Id,
    string Name,
    decimal Price,
    string LastModifiedBy,
    string ImageUri,
    string? Description = null);