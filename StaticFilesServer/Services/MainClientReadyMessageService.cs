using Microsoft.AspNetCore.SignalR;
using LaciSynchroni.Common.SignalR;
using LaciSynchroni.Server.Hubs;

namespace LaciSynchroni.StaticFilesServer.Services;

public class MainClientReadyMessageService : IClientReadyMessageService
{
    private readonly ILogger<MainClientReadyMessageService> _logger;
    private readonly IHubContext<ServerHub> _sinusHub;

    public MainClientReadyMessageService(ILogger<MainClientReadyMessageService> logger, IHubContext<ServerHub> sinusHub)
    {
        _logger = logger;
        _sinusHub = sinusHub;
    }

    public async Task SendDownloadReady(string uid, Guid requestId)
    {
        _logger.LogInformation("Sending Client Ready for {uid}:{requestId} to SignalR", uid, requestId);
        await _sinusHub.Clients.User(uid).SendAsync(nameof(IServerHub.Client_DownloadReady), requestId).ConfigureAwait(false);
    }
}
