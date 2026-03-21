using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpLogging;
using VetPharmacy.Api.Common.Authorization;
using VetPharmacy.Api.Common.ErrorHandling;
using VetPharmacy.Api.Data;
using VetPharmacy.Api.Features.Baskets;
using VetPharmacy.Api.Features.Baskets.Authorization;
using VetPharmacy.Api.Features.Products;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails()
                .AddExceptionHandler<GlobalExceptionHandler>();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.AddVetPharmacyNpgsql<VetPharmacyContext>("VetPharmacyDB");

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.RequestMethod |
                            HttpLoggingFields.RequestPath |
                            HttpLoggingFields.RequestQuery |
                            HttpLoggingFields.ResponseStatusCode |
                            HttpLoggingFields.Duration;
    options.CombineLogs = true;
});

builder.AddVetPharmacyAuthentication();
builder.AddVetPharmacyAuthorization();

builder.Services.AddSingleton<IAuthorizationHandler, BasketAuthorizationHandler>();

//********************************************
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Swagger UI
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "VetPharmacy API v1");
    });
}
else
{
    app.UseExceptionHandler();
}

//app.UseHttpsRedirection();

app.UseHttpLogging();

app.UseStatusCodePages();

// app.UseAuthentication();
// app.UseAuthorization();

app.MapProducts();
app.MapBaskets();

app.Run();