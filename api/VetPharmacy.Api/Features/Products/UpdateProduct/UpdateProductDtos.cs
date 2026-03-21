using System.ComponentModel.DataAnnotations;

namespace VetPharmacy.Api.Features.Products.UpdateProduct;

public record UpdateProductDto(
    [Required][StringLength(250)] string Name,
    [Range(1, 9999)] decimal Price)
{
    [StringLength(500)]
    public string? Description { get; set; }
    public IFormFile? ImageFile { get; set; }
}
