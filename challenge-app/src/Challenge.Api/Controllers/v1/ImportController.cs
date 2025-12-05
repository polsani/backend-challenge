using Challenge.Domain.Contracts.Services;
using Challenge.Domain.Contracts.Storage;
using Microsoft.AspNetCore.Mvc;

namespace Challenge.Api.Controllers.v1;

[ApiController]
[Route("/api/v1/[controller]")]
public class ImportController(IStorageService storageService, IImporterService  importerService)
    : Controller
{
    [HttpGet]
    public async Task<IActionResult> GetPresignedUrlAsync()
    {
        var filename = $"{Guid.NewGuid()}.txt";
        var url = await storageService.GetPresignedPutUrlAsync(filename);

        return Json(new
        {
            url = url.Replace("http://minio", "http://localhost"),
            filename
        });
    }

    [HttpPost]
    public async Task<IActionResult> ImportTransactionsAsync([FromBody] string filename)
    {
        var stream = await storageService.DownloadFileAsync(filename);
        var result = await importerService.ImportFromStreamAsync(stream, filename);
        
        return Ok(result);
    }
}