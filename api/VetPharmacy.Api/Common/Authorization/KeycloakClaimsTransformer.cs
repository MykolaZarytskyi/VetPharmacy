using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace VetPharmacy.Api.Common.Authorization;

public class KeycloakClaimsTransformer()
{
    public void Transform(TokenValidatedContext context)
    {
        var identity = context.Principal?.Identity as ClaimsIdentity;

        identity?.TransformScopeClaim(VetPharmacyClaimTypes.Scope);
        //identity?.MapUserIdClaim(JwtRegisteredClaimNames.Sub);       
    }
}
