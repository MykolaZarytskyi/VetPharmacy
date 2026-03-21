using Microsoft.EntityFrameworkCore;
using VetPharmacy.Api.Data;
using static VetPharmacy.Api.Features.Products.ProductConstants;

namespace VetPharmacy.Api.Features.Products.GetProducts;

public static class GetProductsEndpoint
{
    public static void MapGetProducts(this IEndpointRouteBuilder app)
    {
        // GET /products
        app.MapGet("/", async (
            VetPharmacyContext dbContext,
            [AsParameters] GetProductsDto request,
            CancellationToken cancellationToken) =>
        {
            var skipCount = (request.PageNumber - 1) * request.PageSize;

            var filteredProducts = dbContext.Products
                                .Where(product => string.IsNullOrWhiteSpace(request.Name)
                                        || EF.Functions.Like(product.Name, $"%{request.Name}%"));

            var productsOnPage = await filteredProducts
                                .OrderBy(product => product.Name)
                                .Skip(skipCount)
                                .Take(request.PageSize)
                                .Select(product => new ProductSummaryDto(
                                    product.Id,
                                    product.Name,
                                    product.Price,
                                    product.LastModifiedBy,
                                    product.ImageUri,
                                    product.Description
                                ))
                                .AsNoTracking()
                                .ToListAsync(cancellationToken);

            var totalProducts = await filteredProducts.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling(totalProducts / (double)request.PageSize);

            return new ProductsPageDto(totalPages, productsOnPage);
        })
        .WithName(EndpointNames.GetProducts)
        .WithSummary("Get products")
        .WithDescription("Returns a list of products.")
        .Produces<ProductsPageDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .AllowAnonymous();
    }
}