using VetPharmacy.Api.Data;
using static VetPharmacy.Api.Features.Products.ProductConstants;

namespace VetPharmacy.Api.Features.Products.GetProduct;

public static class GetProductEndpoint
{
    public static void MapGetProduct(this IEndpointRouteBuilder app)
    {
        // GET /products/{id}
        app.MapGet("/{id}", async (
            Guid id,
            VetPharmacyContext dbContext,
            CancellationToken cancellationToken) =>
        {
            var product = await dbContext.Products.FindAsync(id, cancellationToken);

            return product is null ? Results.NotFound() : Results.Ok(
                                new ProductDetailsDto(
                                    product.Id,
                                    product.Name,
                                    product.Price,
                                    product.LastModifiedBy,
                                    product.ImageUri,
                                    product.Description
                                ));
        })
        .WithName(EndpointNames.GetProduct)
        .WithSummary("Get product by ID")
        .WithDescription("Retrieves a product by its unique identifier.")
        .Produces<ProductDetailsDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .AllowAnonymous();
    }
}