using Microsoft.AspNetCore.Authentication.JwtBearer;
using VetPharmacy.Api.Features.Baskets.Authorization;

namespace VetPharmacy.Api.Common.Authorization;

public static class AuthorizationExtensions
{
    private const string ApiAccessScope = "vetpharmacy_api.all";

    public static IHostApplicationBuilder AddVetPharmacyAuthentication(this IHostApplicationBuilder builder)
    {
        var authBuilder = builder.Services.AddAuthentication(Schemes.Keycloak);

        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddSingleton<KeycloakClaimsTransformer>();

            authBuilder.AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters.RoleClaimType = VetPharmacyClaimTypes.Role;
            }).AddJwtBearer(Schemes.Keycloak, options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters.RoleClaimType = VetPharmacyClaimTypes.Role;
                options.RequireHttpsMetadata = false;

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var transformer = context.HttpContext
                                                    .RequestServices
                                                    .GetRequiredService<KeycloakClaimsTransformer>();
                        transformer.Transform(context);

                        return Task.CompletedTask;
                    }
                };
            });
        }

        // authBuilder.AddPolicyScheme(
        //     Schemes.Keycloak,
        //     Schemes.Keycloak,
        //     options =>
        //     {
        //         options.ForwardDefaultSelector = context =>
        //         {
        //             string authHeader = context.Request.Headers[HeaderNames.Authorization]!;

        //             if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        //             {
        //                 var token = authHeader["Bearer ".Length..].Trim();

        //                 var jwtHandler = new JwtSecurityTokenHandler();

        //                 return jwtHandler.CanReadToken(token) &&
        //                         jwtHandler.ReadJwtToken(token)
        //                             .Issuer.Contains("ciamlogin.com")
        //                                 ? Schemes.Entra : Schemes.Keycloak;
        //             }

        //             return Schemes.Entra;
        //         };
        //     });

        return builder;
    }

    public static IHostApplicationBuilder AddVetPharmacyAuthorization(this IHostApplicationBuilder builder)
    {
        builder.Services.AddAuthorizationBuilder()
                        .AddFallbackPolicy(Policies.UserAccess, policy =>
                        {
                            policy.RequireClaim(VetPharmacyClaimTypes.Scope, ApiAccessScope);
                        })
                        .AddPolicy(Policies.OwnerOrAdminBasketAccess, policy =>
                        {
                            policy.Requirements.Add(new OwnerOrAdminRequirement());
                        })
                        .AddPolicy(Policies.AdminAccess, policy =>
                        {
                            policy.RequireClaim(VetPharmacyClaimTypes.Scope, ApiAccessScope);
                            policy.RequireRole(Roles.Admin);
                        });

        return builder;
    }
}
