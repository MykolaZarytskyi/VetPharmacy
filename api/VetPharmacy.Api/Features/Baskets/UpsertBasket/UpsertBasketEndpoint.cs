using Microsoft.EntityFrameworkCore;
using VetPharmacy.Api.Common.Authorization;
using VetPharmacy.Api.Data;
using VetPharmacy.Api.Data.Models;
using static VetPharmacy.Api.Features.Baskets.BasketConstants;

namespace VetPharmacy.Api.Features.Baskets.UpsertBasket;

public static class UpsertBasketEndpoint
{
    public static void MapUpsertBasket(this IEndpointRouteBuilder app)
    {
        // PUT /baskets/{userId}
        app.MapPut("/{userId}", async (
            Guid userId,
            UpsertBasketDto request,
            VetPharmacyContext dbContext,
            CancellationToken cancellationToken) =>
        {
            var basket = await dbContext.Baskets
                            .Include(basket => basket.Items)
                            .FirstOrDefaultAsync(basket => basket.Id == userId, cancellationToken);

            if (basket is null)
            {
                basket = new CustomerBasket
                {
                    Id = userId,
                    Items = [.. request.Items.Select(item => new BasketItem
                    {
                        ProductId = item.Id,
                        Quantity = item.Quantity
                    })]
                };

                dbContext.Baskets.Add(basket);
            }
            else
            {
                basket.Items = [.. request.Items.Select(item => new BasketItem
                {
                    ProductId = item.Id,
                    Quantity = item.Quantity
                })];
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            return Results.NoContent();
        })
        .WithName(EndpointNames.UpsertBasket)
        .WithSummary("Upsert basket")
        .WithDescription("Upserts a basket for a user.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .WithParameterValidation()
        .DisableAntiforgery()
        .RequireAuthorization(Policies.OwnerOrAdminBasketAccess);
    }
}