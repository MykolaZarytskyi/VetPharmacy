using VetPharmacy.Api.Features.Baskets.GetBasket;
using VetPharmacy.Api.Features.Baskets.UpsertBasket;

namespace VetPharmacy.Api.Features.Baskets;

public static class BasketsEndpoints
{
    public static void MapBaskets(this WebApplication app)
    {
        var group = app.MapGroup("/baskets")
        .WithTags("Baskets");

        group.MapUpsertBasket();
        group.MapGetBasket();
    }
}
