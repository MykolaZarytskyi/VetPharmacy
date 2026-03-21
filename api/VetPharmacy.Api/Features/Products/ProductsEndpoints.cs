using VetPharmacy.Api.Features.Products.CreateProduct;
using VetPharmacy.Api.Features.Products.DeleteProduct;
using VetPharmacy.Api.Features.Products.GetProduct;
using VetPharmacy.Api.Features.Products.GetProducts;
using VetPharmacy.Api.Features.Products.UpdateProduct;

namespace VetPharmacy.Api.Features.Products;

public static class ProductsEndpoints
{
    public static void MapProducts(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/products")
        .WithTags("Products");

        group.MapGetProducts();
        group.MapGetProduct();
        group.MapCreateProduct();
        group.MapDeleteProduct();
        group.MapUpdateProduct();
    }
}