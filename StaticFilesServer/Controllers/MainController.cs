using LaciSynchroni.Common.Routes;
using LaciSynchroni.Shared.Utils.Configuration;
using LaciSynchroni.StaticFilesServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LaciSynchroni.StaticFilesServer.Controllers;

[Route(FilesRoutes.Main)]
[Authorize(Policy = "Internal")]
public class MainController : ControllerBase
{
    private readonly IClientReadyMessageService _messageService;
    private readonly MainServerShardRegistrationService _shardRegistrationService;

    public MainController(ILogger<MainController> logger, IClientReadyMessageService hub,
        MainServerShardRegistrationService shardRegistrationService) : base(logger)
    {
        _messageService = hub;
        _shardRegistrationService = shardRegistrationService;
    }

    [HttpGet(FilesRoutes.Main_SendReady)]
    public async Task<IActionResult> SendReadyToClients(string uid, Guid requestId)
    {
        await _messageService.SendDownloadReady(uid, requestId).ConfigureAwait(false);
        return Ok();
    }

    [HttpPost("shardRegister")]
    public IActionResult RegisterShard([FromBody] ShardConfiguration shardConfiguration)
    {
        try
        {
            _shardRegistrationService.RegisterShard(LaciUser, shardConfiguration);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Shard could not be registered {shard}", LaciUser);
            return BadRequest();
        }
    }

    [HttpPost("shardUnregister")]
    public IActionResult UnregisterShard()
    {
        _shardRegistrationService.UnregisterShard(LaciUser);
        return Ok();
    }

    [HttpPost("shardHeartbeat")]
    public IActionResult ShardHeartbeat()
    {
        try
        {
            _shardRegistrationService.ShardHeartbeat(LaciUser);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Shard not registered: {shard}", LaciUser);
            return BadRequest();
        }
    }
}