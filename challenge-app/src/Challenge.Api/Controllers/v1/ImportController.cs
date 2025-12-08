using Challenge.Api.Filters;
using Challenge.Domain.Contracts.Services;
using Challenge.Domain.Contracts.Storage;
using Challenge.Domain.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;

namespace Challenge.Api.Controllers.v1;

[ApiController]
[Route("/api/v1/[controller]")]
public class ImportController(IStorageService storageService, IImporterService  importerService, 
    ILogger<ImportController> logger) : Controller
{
    [HttpGet]
    public async Task<IActionResult> GetPresignedUrlAsync()
    {
        var filename = $"{Guid.NewGuid()}";
        var url = await storageService.GetPresignedPutUrlAsync(filename);

        return Json(new
        {
            url,
            filename
        });
    }

    [HttpPost]
    [ServiceFilter(typeof(MemoryOptimizationFilter))]
    public async Task<IActionResult> ImportTransactionsAsync([FromBody] ImportRequest importRequest)
    {
        var startDate = DateTimeOffset.UtcNow;
        ImportResult? result = null;
        
        await storageService.DownloadFileStreamAsync(importRequest.FileName, async (x, _) =>
        {
            try
            {
                result = await importerService.ImportFromStreamAsync(x, importRequest.FileName, importRequest.ShowSummary);
            }
            catch (Exception e)
            {
                logger.LogError(e, "An error occured while importing transactions");
            }

            return result;
        });
        
        var finishDate = DateTimeOffset.UtcNow;
        var elapsed = (finishDate - startDate).Seconds;
        logger.LogDebug("Elapsed time: {elapsed-seconds} seconds", elapsed);
        
        return Ok(result);
    }
}