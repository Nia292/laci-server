using Microsoft.AspNetCore.SignalR;
using LaciSynchroni.Common.SignalR;
using LaciSynchroni.Server.Hubs;

namespace LaciSynchroni.StaticFilesServer.Services;

public class MainClientReadyMessageService : IClientReadyMessageService
{
    private readonly ILogger<MainClientReadyMessageService> _logger;
    private readonly IHubContext<ServerHub> _hub;

    public MainClientReadyMessageService(ILogger<MainClientReadyMessageService> logger, IHubContext<ServerHub> hub)
    {
        _logger = logger;
        _hub = hub;
    }

    public async Task SendDownloadReady(string uid, Guid requestId)
    {
        _logger.LogInformation("Sending Client Ready for {uid}:{requestId} to SignalR", uid, requestId);
        await _hub.Clients.User(uid).SendAsync(nameof(IServerHub.Client_DownloadReady), requestId).ConfigureAwait(false);
    }
}
