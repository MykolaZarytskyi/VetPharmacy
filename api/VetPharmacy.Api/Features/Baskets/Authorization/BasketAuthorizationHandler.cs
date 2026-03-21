using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using VetPharmacy.Api.Common.Authorization;

namespace VetPharmacy.Api.Features.Baskets.Authorization;

// Authorization handler. A class responsible for the evaluation of 
// a requirement's properties.

// A resource-based handler is an authorization handler that specifies 
// both a requirement and a resource type.

public class BasketAuthorizationHandler
    : AuthorizationHandler<OwnerOrAdminRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OwnerOrAdminRequirement requirement)
    {
        var currentUserId = context.User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (string.IsNullOrEmpty(currentUserId))
        {
            context.Fail();
            return Task.CompletedTask;
        }

        var userId = string.Empty;

        if (context.Resource is HttpContext httpContext)
        {
            userId = httpContext.Request.RouteValues["userId"]?.ToString();
        }

        if ((!string.IsNullOrWhiteSpace(userId) && Guid.Parse(currentUserId) == Guid.Parse(userId))
            || context.User.IsInRole(Roles.Admin))
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }

        return Task.CompletedTask;
    }
}

// Authorization requirement. A collection of data parameters that a policy 
// can use to evaluate the current user principal.
public class OwnerOrAdminRequirement : IAuthorizationRequirement { }