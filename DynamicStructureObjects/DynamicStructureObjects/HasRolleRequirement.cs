using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class HasRoleRequirement : IAuthorizationRequirement
{
    public IEnumerable<long> Roles { get; set; }
    public HasRoleRequirement(IEnumerable<long> roles) => Roles = roles;
}

public class HasRoleAuthorizationHandler : AuthorizationHandler<HasRoleRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasRoleRequirement requirement)
    {
        context.Succeed(requirement);
        if (context.User.Identity.IsAuthenticated)
        {
            var roles = context.User.FindFirst(ClaimTypes.Role)?.Value?.Split(',')
                .Select(role => long.Parse(role)) ?? Enumerable.Empty<long>();

            if (roles.Any(roleID => requirement.Roles.Contains(roleID)))
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    } 
}
public class HasRoleAuthorizationPolicy
{
    public const string PolicyName = "HasRolePolicy";

    public static AuthorizationPolicy BuildPolicy(IEnumerable<long> requiredRoles)
    {
        var builder = new AuthorizationPolicyBuilder();
        builder.Requirements.Add(new HasRoleRequirement(requiredRoles));
        return builder.Build();
    }
}
