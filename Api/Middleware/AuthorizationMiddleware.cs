using Domain.Model.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace Api.Middleware;

public static class AuthorizationMiddleware
{
    public static TBuilder CustomAuthorization<TBuilder>(this TBuilder builder, UserRole role)
        where TBuilder : IEndpointConventionBuilder

    {
        return CustomAuthorization(builder, [role]);
    }

    public static TBuilder CustomAuthorization<TBuilder>(this TBuilder builder, IEnumerable<UserRole> roles)
        where TBuilder : IEndpointConventionBuilder

    {
        builder.RequireAuthorization(new AuthorizeAttribute()
            { Roles = string.Join(",", roles.Select(x => x.ToString())) });
        return builder;
    }
}