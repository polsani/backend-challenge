using Challenge.Domain.Contracts.Storage;
using Microsoft.AspNetCore.Mvc;

namespace Challenge.Api.Controllers.v1;

[ApiController]
[Route("/api/v1/[controller]")]
public class ImportController : Controller
{
    private readonly IStorageService _storageService;
    private readonly ILogger<ImportController> _logger;

    public ImportController(IStorageService storageService, ILogger<ImportController> logger)
    {
        _storageService = storageService;
        _logger = logger;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var url = await _storageService.GetPresignedUrlAsync(Guid.NewGuid().ToString());
        _logger.LogDebug("Generating new presignedUrl: {Url}", url);
        return Json(url);
    }
}