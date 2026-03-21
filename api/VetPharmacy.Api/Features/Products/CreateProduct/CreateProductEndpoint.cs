using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using VetPharmacy.Api.Common.Authorization;
using VetPharmacy.Api.Data;
using VetPharmacy.Api.Data.Models;
using static VetPharmacy.Api.Features.Products.ProductConstants;

namespace VetPharmacy.Api.Features.Products.CreateProduct;

public static class CreateProductEndpoint
{
    private const string DefaultImageUri = "https://placehold.co/100";

    public static void MapCreateProduct(this IEndpointRouteBuilder app)
    {
        // POST /products
        app.MapPost("/", HandleCreateProduct)
        .WithName(EndpointNames.CreateProduct)
        .WithSummary("Create product")
        .WithDescription("Creates a new product.")
        .Produces<ProductDetailsDto>(StatusCodes.Status201Created)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .WithParameterValidation()
        .DisableAntiforgery()
        .RequireAuthorization(Policies.AdminAccess);
        //.RequireAuthorization(builder => builder.RequireRole(Roles.Admin));
    }

    public static async Task<IResult> HandleCreateProduct(
        [FromForm] CreateProductDto productDto,
        VetPharmacyContext dbContext,
        ILogger<Program> logger,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        if (user?.Identity?.IsAuthenticated == false)
        {
            return Results.Unauthorized();
        }

        var currentUserIdentifier = user?.FindFirstValue(JwtRegisteredClaimNames.Email)
                                    ?? user?.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (string.IsNullOrEmpty(currentUserIdentifier))
        {
            return Results.Unauthorized();
        }

        var imageUri = DefaultImageUri;

        var product = new Product
        {
            Name = productDto.Name,
            Price = productDto.Price,
            Description = productDto.Description,
            ImageUri = imageUri,
            LastModifiedBy = currentUserIdentifier
        };

        dbContext.Products.Add(product);

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Created product {ProductName} with price {ProductPrice}",
            product.Name,
            product.Price);

        return Results.CreatedAtRoute(
            EndpointNames.GetProduct,
            new { id = product.Id },
            new ProductDetailsDto(
                product.Id,
                product.Name,
                product.Price,
                product.LastModifiedBy,
                product.ImageUri,
                product.Description
            ));
    }
}