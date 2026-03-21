namespace VetPharmacy.Api.Data.Models;

public class BasketItem
{
    public Guid Id { get; set; }

    public Product? Product { get; set; }

    public Guid ProductId { get; set; }

    public int Quantity { get; set; }

    public Guid CustomerBasketId { get; set; }
}
