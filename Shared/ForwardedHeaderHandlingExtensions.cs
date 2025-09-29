using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LaciSynchroni.Shared;

public static class ForwardedHeaderHandlingExtensions
{
    public static IApplicationBuilder UseCustomIpAddressHandling(this IApplicationBuilder builder)
    {
        const string middlewareAdded = $"{nameof(CustomForwardedHeaderHandlingMiddleware)}Added";

        ArgumentNullException.ThrowIfNull(builder);

        if (!builder.Properties.ContainsKey(middlewareAdded))
        {
            builder.Properties[middlewareAdded] = true;
            builder.UseMiddleware<CustomForwardedHeaderHandlingMiddleware>();
        }

        builder.UseForwardedHeaders();
        return builder;
    }

    public static IPAddress? GetClientIpAddress(this HttpContext httpContext)
    {
        return httpContext.Connection.RemoteIpAddress;
    }

    public static IPAddress? GetPeerIpAddress(this HttpContext httpContext)
    {
        var options = httpContext
            .RequestServices.GetRequiredService<IOptions<ForwardedHeadersOptions>>()
            .Value;

        if (
            httpContext.Request.Headers.TryGetValue(options.OriginalForHeaderName, out var xof)
            && xof is [{ } original]
            && IPEndPoint.TryParse(original, out var endpoint)
        )
        {
            return endpoint.Address;
        }

        return httpContext.GetClientIpAddress();
    }
}
