using Microsoft.EntityFrameworkCore;
using VetPharmacy.Api.Data.Configurations;
using VetPharmacy.Api.Data.Models;

namespace VetPharmacy.Api.Data;

public class VetPharmacyContext(DbContextOptions<VetPharmacyContext> options)
    : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    public DbSet<CustomerBasket> Baskets => Set<CustomerBasket>();

    public DbSet<BasketItem> BasketItems => Set<BasketItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductEntityConfiguration).Assembly);
    }
}