using LaciSynchroni.Common.SignalR;
using LaciSynchroni.Server.Hubs;
using LaciSynchroni.Shared.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace LaciSynchroni.Server.Controllers;

[Route("/msgc")]
[Authorize(Policy = "Internal")]
public class ClientMessageController : Controller
{
    private ILogger<ClientMessageController> _logger;
    private IHubContext<ServerHub, IServerHub> _hubContext;

    public ClientMessageController(ILogger<ClientMessageController> logger, IHubContext<ServerHub, IServerHub> hubContext)
    {
        _logger = logger;
        _hubContext = hubContext;
    }

    [Route("sendMessage")]
    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] ClientMessage msg)
    {
        bool hasUid = !string.IsNullOrEmpty(msg.UID);

        if (!hasUid)
        {
            _logger.LogInformation("Sending Message of severity {severity} to all online users: {message}", msg.Severity, msg.Message);
            await _hubContext.Clients.All.Client_ReceiveServerMessage(msg.Severity, msg.Message).ConfigureAwait(false);
        }
        else
        {
            _logger.LogInformation("Sending Message of severity {severity} to user {uid}: {message}", msg.Severity, msg.UID, msg.Message);
            await _hubContext.Clients.User(msg.UID).Client_ReceiveServerMessage(msg.Severity, msg.Message).ConfigureAwait(false);
        }

        return Empty;
    }
}
