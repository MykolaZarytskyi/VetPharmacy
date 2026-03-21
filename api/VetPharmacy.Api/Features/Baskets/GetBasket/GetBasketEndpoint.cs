using Microsoft.EntityFrameworkCore;
using VetPharmacy.Api.Common.Authorization;
using VetPharmacy.Api.Data;
using static VetPharmacy.Api.Features.Baskets.BasketConstants;

namespace VetPharmacy.Api.Features.Baskets.GetBasket;

public static class GetBasketEndpoint
{
    public static void MapGetBasket(this IEndpointRouteBuilder app)
    {
        // Get /baskets/{userId}
        app.MapGet("/{userId}", async (
            Guid userId,
            VetPharmacyContext dbContext
        ) =>
        {
            if (userId == Guid.Empty)
            {
                return Results.BadRequest();
            }

            var basket = await dbContext.Baskets
                                        .Include(basket => basket.Items)
                                        .ThenInclude(item => item.Product)
                                        .FirstOrDefaultAsync(
                                            basket => basket.Id == userId)
                                            ?? new() { Id = userId }; // Return empty basket if not found

            var dto = new BasketDto(
                basket.Id,
                basket.Items.Select(item => new BasketItemDto(
                    item.ProductId,
                    item.Product!.Name,
                    item.Product!.Price,
                    item.Quantity,
                    item.Product!.ImageUri
                ))
                .OrderBy(item => item.Name));

            return Results.Ok(dto);
        })
        .WithName(EndpointNames.GetBasket)
        .WithSummary("Get basket by user ID")
        .WithDescription("Retrieves a basket for a specific user.")
        .Produces<BasketDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .RequireAuthorization(Policies.OwnerOrAdminBasketAccess);
        //.RequireAuthorization(builder => builder.RequireClaim("scope", "basket.read"));
    }
}
