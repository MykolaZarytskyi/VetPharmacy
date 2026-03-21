using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using VetPharmacy.Api.Data;
using VetPharmacy.Api.Features.Products.CreateProduct;
using static VetPharmacy.Api.Features.Products.ProductConstants;
using FluentAssertions;

namespace VetPharmacy.Api.Tests;

public class CreateProductEndpointTests
{
    [Fact]
    public async Task HandleCreateProduct_ReturnsCreated_WhenValidRequest()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<Program>>();
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Email, "test@example.com")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        var options = new DbContextOptionsBuilder<VetPharmacyContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new VetPharmacyContext(options);

        var dto = new CreateProductDto("Test Product", 10.99m) { Description = "Test Description" };

        // Act
        var result = await CreateProductEndpoint.HandleCreateProduct(dto, context, mockLogger.Object, user, CancellationToken.None);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtRoute<ProductDetailsDto>>().Subject;
        createdResult.RouteName.Should().Be(EndpointNames.GetProduct);
        createdResult.Value.Should().NotBeNull();
        createdResult.Value.Name.Should().Be(dto.Name);
        createdResult.Value.Price.Should().Be(dto.Price);
        createdResult.Value.Description.Should().Be(dto.Description);
        createdResult.Value.LastModifiedBy.Should().Be("test@example.com");
        createdResult.Value.ImageUri.Should().Be("https://placehold.co/100");

        // Verify product was added to context
        var product = await context.Products.FirstOrDefaultAsync(p => p.Name == dto.Name);
        product.Should().NotBeNull();
        product.Name.Should().Be(dto.Name);
        product.Price.Should().Be(dto.Price);
        product.Description.Should().Be(dto.Description);
        product.LastModifiedBy.Should().Be("test@example.com");
    }

    [Fact]
    public async Task HandleCreateProduct_ReturnsUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<Program>>();
        var user = new ClaimsPrincipal(); // Not authenticated

        var options = new DbContextOptionsBuilder<VetPharmacyContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new VetPharmacyContext(options);

        var dto = new CreateProductDto("Test Product", 10.99m);

        // Act
        var result = await CreateProductEndpoint.HandleCreateProduct(dto, context, mockLogger.Object, user, CancellationToken.None);

        // Assert
        result.Should().BeOfType<UnauthorizedHttpResult>();
    }

    [Fact]
    public async Task HandleCreateProduct_ReturnsUnauthorized_WhenNoUserIdentifier()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<Program>>();
        var claims = new List<Claim>(); // No email or sub
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        var options = new DbContextOptionsBuilder<VetPharmacyContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new VetPharmacyContext(options);

        var dto = new CreateProductDto("Test Product", 10.99m);

        // Act
        var result = await CreateProductEndpoint.HandleCreateProduct(dto, context, mockLogger.Object, user, CancellationToken.None);

        // Assert
        result.Should().BeOfType<UnauthorizedHttpResult>();
    }

    [Fact]
    public async Task HandleCreateProduct_LogsInformation_WhenProductCreated()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<Program>>();
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, "user123")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        var options = new DbContextOptionsBuilder<VetPharmacyContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new VetPharmacyContext(options);

        var dto = new CreateProductDto("Test Product", 15.50m);

        // Act
        await CreateProductEndpoint.HandleCreateProduct(dto, context, mockLogger.Object, user, CancellationToken.None);

        // Assert
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Created product Test Product with price 15.5")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}