using LaciSynchroni.Shared.Metrics;
using LaciSynchroni.StaticFilesServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace LaciSynchroni.StaticFilesServer.Utils;

public class RequestFileStreamResult : FileStreamResult
{
    private readonly Guid _requestId;
    private readonly RequestQueueService _requestQueueService;
    private readonly LaciMetrics _metrics;

    public RequestFileStreamResult(Guid requestId, RequestQueueService requestQueueService, LaciMetrics metrics,
        Stream fileStream, string contentType) : base(fileStream, contentType)
    {
        _requestId = requestId;
        _requestQueueService = requestQueueService;
        _metrics = metrics;
        _metrics.IncGauge(MetricsAPI.GaugeCurrentDownloads);
    }

    public override void ExecuteResult(ActionContext context)
    {
        try
        {
            base.ExecuteResult(context);
        }
        catch
        {
            throw;
        }
        finally
        {
            _requestQueueService.FinishRequest(_requestId);

            _metrics.DecGauge(MetricsAPI.GaugeCurrentDownloads);
            FileStream?.Dispose();
        }
    }

    public override async Task ExecuteResultAsync(ActionContext context)
    {
        try
        {
            await base.ExecuteResultAsync(context).ConfigureAwait(false);
        }
        catch
        {
            throw;
        }
        finally
        {
            _requestQueueService.FinishRequest(_requestId);
            _metrics.DecGauge(MetricsAPI.GaugeCurrentDownloads);
            FileStream?.Dispose();
        }
    }
}