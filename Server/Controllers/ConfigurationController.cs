using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LaciSynchroni.Common.Dto.Server;
using LaciSynchroni.Shared.Services;
using LaciSynchroni.Shared.Utils.Configuration;
using System.Reflection;

namespace LaciSynchroni.Server.Controllers;

[ApiController]
[Route("/configuration")]
[AllowAnonymous]
public class ConfigurationController(
    IConfigurationService<ServerConfiguration> serverConfig) : ControllerBase
{
    [Route("get")]
    public IActionResult GetConfiguration()
    {
        var configuration = new ConfigurationDto()
        {
            ServerName = serverConfig.GetValueOrDefault(nameof(ServerConfiguration.ServerName), "Unnamed Server"),
            ServerVersion = Assembly.GetExecutingAssembly().GetName().Version,
            ServerUri = serverConfig.GetValueOrDefault(nameof(ServerConfiguration.ServerPublicUri), new Uri("wss://noemptyuri")),
            DiscordInvite = serverConfig.GetValueOrDefault<string>(nameof(ServerConfiguration.DiscordInvite), defaultValue: null),
            ServerRules = serverConfig.GetValueOrDefault<string>(nameof(ServerConfiguration.ServerRules), defaultValue: null),
        };

        return Ok(configuration);
    }
}