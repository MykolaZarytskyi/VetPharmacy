using Microsoft.EntityFrameworkCore;
using VetPharmacy.Api.Common.Authorization;
using VetPharmacy.Api.Data;
using static VetPharmacy.Api.Features.Products.ProductConstants;

namespace VetPharmacy.Api.Features.Products.DeleteProduct;

public static class DeleteProductEndpoint
{
    public static void MapDeleteProduct(this IEndpointRouteBuilder app)
    {
        // DELETE /products/{id}
        app.MapDelete("/{id}", async (Guid id, VetPharmacyContext dbContext, CancellationToken cancellationToken) =>
        {
            await dbContext.Products
                     .Where(product => product.Id == id)
                     .ExecuteDeleteAsync(cancellationToken);

            return Results.NoContent();
        })
        .WithName(EndpointNames.DeleteProduct)
        .WithSummary("Delete product")
        .WithDescription("Deletes a product by its identifier.")
        .Produces(StatusCodes.Status204NoContent)
        .RequireAuthorization(Policies.AdminAccess);
    }
}