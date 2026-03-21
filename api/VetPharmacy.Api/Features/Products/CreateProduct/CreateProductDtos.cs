using System.ComponentModel.DataAnnotations;

namespace VetPharmacy.Api.Features.Products.CreateProduct;

public record CreateProductDto(
    [Required][StringLength(250)] string Name,
    [Range(1, 9999)] decimal Price)
{
    [StringLength(500)]
    public string? Description { get; set; }
    public IFormFile? ImageFile { get; set; }
}

public record ProductDetailsDto(
    Guid Id,
    string Name,
    decimal Price,
    string LastModifiedBy,
    string ImageUri,
    string? Description = null);