using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace LaciSynchroni.Shared;

public class CustomForwardedHeaderHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IOptions<ForwardedHeadersOptions> _options;

    public CustomForwardedHeaderHandlingMiddleware(
        RequestDelegate next,
        IOptions<ForwardedHeadersOptions> options
    )
    {
        _next = next;
        _options = options;
    }

    public Task Invoke(HttpContext httpContext)
    {
        httpContext.Request.Headers[_options.Value.OriginalForHeaderName] = [];
        return _next(httpContext);
    }
}
