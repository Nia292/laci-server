using LaciSynchroni.Shared.Utils.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LaciSynchroni.Shared.Services;

[Route("configuration/[controller]")]
[Authorize(Policy = "Internal")]
public class LaciConfigurationController<T> : Controller where T : class, ILaciConfiguration
{
    private readonly ILogger<LaciConfigurationController<T>> _logger;
    private IOptionsMonitor<T> _config;

    public LaciConfigurationController(IOptionsMonitor<T> config, ILogger<LaciConfigurationController<T>> logger)
    {
        _config = config;
        _logger = logger;
    }

    [HttpGet("GetConfigurationEntry")]
    [Authorize(Policy = "Internal")]
    public IActionResult GetConfigurationEntry(string key, string defaultValue)
    {
        var result = _config.CurrentValue.SerializeValue(key, defaultValue);
        _logger.LogInformation("Requested " + key + ", returning:" + result);
        return Ok(result);
    }
}

#pragma warning disable MA0048 // File name must match type name
public class LaciStaticFilesServerConfigurationController : LaciConfigurationController<StaticFilesServerConfiguration>
{
    public LaciStaticFilesServerConfigurationController(IOptionsMonitor<StaticFilesServerConfiguration> config, ILogger<LaciStaticFilesServerConfigurationController> logger) : base(config, logger)
    {
    }
}

public class LaciBaseConfigurationController : LaciConfigurationController<LaciConfigurationBase>
{
    public LaciBaseConfigurationController(IOptionsMonitor<LaciConfigurationBase> config, ILogger<LaciBaseConfigurationController> logger) : base(config, logger)
    {
    }
}

public class LaciServerConfigurationController : LaciConfigurationController<ServerConfiguration>
{
    public LaciServerConfigurationController(IOptionsMonitor<ServerConfiguration> config, ILogger<LaciServerConfigurationController> logger) : base(config, logger)
    {
    }
}

public class LaciServicesConfigurationController : LaciConfigurationController<ServicesConfiguration>
{
    public LaciServicesConfigurationController(IOptionsMonitor<ServicesConfiguration> config, ILogger<LaciServicesConfigurationController> logger) : base(config, logger)
    {
    }
}
#pragma warning restore MA0048 // File name must match type name
