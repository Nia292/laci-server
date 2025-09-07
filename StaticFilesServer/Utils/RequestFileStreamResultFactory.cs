using LaciSynchroni.Shared.Metrics;
using LaciSynchroni.Shared.Services;
using LaciSynchroni.Shared.Utils.Configuration;
using LaciSynchroni.StaticFilesServer.Services;

namespace LaciSynchroni.StaticFilesServer.Utils;

public class RequestFileStreamResultFactory
{
    private readonly SinusMetrics _metrics;
    private readonly RequestQueueService _requestQueueService;
    private readonly IConfigurationService<StaticFilesServerConfiguration> _configurationService;

    public RequestFileStreamResultFactory(SinusMetrics metrics, RequestQueueService requestQueueService, IConfigurationService<StaticFilesServerConfiguration> configurationService)
    {
        _metrics = metrics;
        _requestQueueService = requestQueueService;
        _configurationService = configurationService;
    }

    public RequestFileStreamResult Create(Guid requestId, Stream stream)
    {
        return new RequestFileStreamResult(requestId, _requestQueueService,
            _metrics, stream, "application/octet-stream");
    }
}