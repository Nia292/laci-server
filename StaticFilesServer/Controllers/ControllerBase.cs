using LaciSynchroni.Shared.Utils;
using Microsoft.AspNetCore.Mvc;

namespace LaciSynchroni.StaticFilesServer.Controllers;

public class ControllerBase : Controller
{
    protected ILogger _logger;

    public ControllerBase(ILogger logger)
    {
        _logger = logger;
    }

    protected string LaciUser => HttpContext.User.Claims.First(f => string.Equals(f.Type, LaciClaimTypes.Uid, StringComparison.Ordinal)).Value;
    protected string Continent => HttpContext.User.Claims.FirstOrDefault(f => string.Equals(f.Type, LaciClaimTypes.Continent, StringComparison.Ordinal))?.Value ?? "*";
    protected bool IsPriority => !string.IsNullOrEmpty(HttpContext.User.Claims.FirstOrDefault(f => string.Equals(f.Type, LaciClaimTypes.Alias, StringComparison.Ordinal))?.Value ?? string.Empty);
}
