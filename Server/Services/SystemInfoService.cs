using LaciSynchroni.Common.Dto;
using LaciSynchroni.Common.SignalR;
using LaciSynchroni.Server.Hubs;
using LaciSynchroni.Shared.Data;
using LaciSynchroni.Shared.Metrics;
using LaciSynchroni.Shared.Services;
using LaciSynchroni.Shared.Utils.Configuration;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace LaciSynchroni.Server.Services;

public sealed class SystemInfoService : BackgroundService
{
    private readonly SinusMetrics _sinusMetrics;
    private readonly IConfigurationService<ServerConfiguration> _config;
    private readonly IDbContextFactory<SinusDbContext> _dbContextFactory;
    private readonly ILogger<SystemInfoService> _logger;
    private readonly IHubContext<ServerHub, IServerHub> _hubContext;
    private readonly IRedisDatabase _redis;
    public SystemInfoDto SystemInfoDto { get; private set; } = new();

    public SystemInfoService(SinusMetrics sinusMetrics, IConfigurationService<ServerConfiguration> configurationService, IDbContextFactory<SinusDbContext> dbContextFactory,
        ILogger<SystemInfoService> logger, IHubContext<ServerHub, IServerHub> hubContext, IRedisDatabase redisDb)
    {
        _sinusMetrics = sinusMetrics;
        _config = configurationService;
        _dbContextFactory = dbContextFactory;
        _logger = logger;
        _hubContext = hubContext;
        _redis = redisDb;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await base.StartAsync(cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("System Info Service started");
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        var timeOut = _config.IsMain ? 15 : 30;

        while (!ct.IsCancellationRequested)
        {
            try
            {
                ThreadPool.GetAvailableThreads(out int workerThreads, out int ioThreads);

                _sinusMetrics.SetGaugeTo(MetricsAPI.GaugeAvailableWorkerThreads, workerThreads);
                _sinusMetrics.SetGaugeTo(MetricsAPI.GaugeAvailableIOWorkerThreads, ioThreads);

                var onlineUsers = (_redis.SearchKeysAsync("UID:*").GetAwaiter().GetResult()).Count();
                SystemInfoDto = new SystemInfoDto()
                {
                    OnlineUsers = onlineUsers,
                };

                if (_config.IsMain)
                {
                    _logger.LogInformation("Sending System Info, Online Users: {onlineUsers}", onlineUsers);

                    await _hubContext.Clients.All.Client_UpdateSystemInfo(SystemInfoDto).ConfigureAwait(false);

                    using var db = await _dbContextFactory.CreateDbContextAsync(ct).ConfigureAwait(false);

                    _sinusMetrics.SetGaugeTo(MetricsAPI.GaugeAuthorizedConnections, onlineUsers);
                    _sinusMetrics.SetGaugeTo(MetricsAPI.GaugePairs, db.ClientPairs.AsNoTracking().Count());
                    _sinusMetrics.SetGaugeTo(MetricsAPI.GaugePairsPaused, db.Permissions.AsNoTracking().Where(p => p.IsPaused).Count());
                    _sinusMetrics.SetGaugeTo(MetricsAPI.GaugeGroups, db.Groups.AsNoTracking().Count());
                    _sinusMetrics.SetGaugeTo(MetricsAPI.GaugeGroupPairs, db.GroupPairs.AsNoTracking().Count());
                    _sinusMetrics.SetGaugeTo(MetricsAPI.GaugeUsersRegistered, db.Users.AsNoTracking().Count());
                }

                await Task.Delay(TimeSpan.FromSeconds(timeOut), ct).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to push system info");
            }
        }
    }
}