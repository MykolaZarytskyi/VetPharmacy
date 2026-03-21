namespace VetPharmacy.Api.Data.Models;

public class Product
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required decimal Price { get; set; }
    public required string ImageUri { get; set; }
    public required string LastModifiedBy { get; set; }
}