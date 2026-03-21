using System.Security.Claims;

namespace VetPharmacy.Api.Common.Authorization;

public static class ClaimsExtensions
{
    public static void TransformScopeClaim(
        this ClaimsIdentity? identity,
        string sourceScopeClaimType)
    {
        var scopeClaim = identity?.FindFirst(sourceScopeClaimType);

        if (scopeClaim is null)
        {
            return;
        }

        var scopes = scopeClaim.Value.Split(' ');

        identity?.RemoveClaim(scopeClaim);

        identity?.AddClaims(
            scopes.Select(
                scope => new Claim(VetPharmacyClaimTypes.Scope, scope)));
    }
}
