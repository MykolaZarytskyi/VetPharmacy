using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetPharmacy.Api.Common.Authorization;
using VetPharmacy.Api.Data;
using static VetPharmacy.Api.Features.Products.ProductConstants;

namespace VetPharmacy.Api.Features.Products.UpdateProduct;

public static class UpdateProductEndpoint
{
    public static void MapUpdateProduct(this IEndpointRouteBuilder app)
    {
        // PUT /products/{id}
        app.MapPut("/{id}", async (
            Guid id,
            [FromForm] UpdateProductDto request,
            ClaimsPrincipal user,
            VetPharmacyContext dbContext) =>
        {
            var currentUserIdentifier = user?.FindFirstValue(JwtRegisteredClaimNames.Email)
                                        ?? user?.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (string.IsNullOrEmpty(currentUserIdentifier))
            {
                return Results.Unauthorized();
            }

            var rows = await dbContext.Products
            .Where(p => p.Id == id)
            .ExecuteUpdateAsync(p => p
                .SetProperty(p => p.Name, request.Name)
                .SetProperty(p => p.Price, request.Price)
                .SetProperty(p => p.Description, request.Description)
                .SetProperty(p => p.LastModifiedBy, currentUserIdentifier)
            );

            return rows == 0
            ? Results.NotFound()
            : Results.NoContent();
        })
        .WithName(EndpointNames.UpdateProduct)
        .WithSummary("Update product")
        .WithDescription("Updates an existing product.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .WithParameterValidation()
        .DisableAntiforgery()
        .RequireAuthorization(Policies.AdminAccess);
    }
}
