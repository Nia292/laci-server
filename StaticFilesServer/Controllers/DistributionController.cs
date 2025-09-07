using LaciSynchroni.Common.Routes;
using LaciSynchroni.StaticFilesServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LaciSynchroni.StaticFilesServer.Controllers;

[Route(FilesRoutes.Distribution)]
public class DistributionController : ControllerBase
{
    private readonly CachedFileProvider _cachedFileProvider;

    public DistributionController(ILogger<DistributionController> logger, CachedFileProvider cachedFileProvider) : base(logger)
    {
        _cachedFileProvider = cachedFileProvider;
    }

    [HttpGet(FilesRoutes.Distribution_Get)]
    [Authorize(Policy = "Internal")]
    public async Task<IActionResult> GetFile(string file)
    {
        _logger.LogInformation($"GetFile:{LaciUser}:{file}");

        var fs = await _cachedFileProvider.DownloadAndGetLocalFileInfo(file);
        if (fs == null) return NotFound();

        return PhysicalFile(fs.FullName, "application/octet-stream");
    }
}
