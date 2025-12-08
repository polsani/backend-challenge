using System.Text;
using Challenge.Api.Controllers.v1;
using Challenge.Domain.Contracts.Services;
using Challenge.Domain.Contracts.Storage;
using Challenge.Domain.DataTransferObjects;
using Challenge.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Challenge.Api.Tests.Controllers;

public class ImportControllerTests
{
    private readonly Mock<IStorageService> _storageServiceMock;
    private readonly Mock<IImporterService> _importerServiceMock;
    private readonly Mock<ILogger<ImportController>> _loggerMock;
    private readonly ImportController _controller;

    public ImportControllerTests()
    {
        _storageServiceMock = new Mock<IStorageService>();
        _importerServiceMock = new Mock<IImporterService>();
        _loggerMock = new Mock<ILogger<ImportController>>();
        
        _controller = new ImportController(_storageServiceMock.Object, _importerServiceMock.Object, 
            _loggerMock.Object);
    }

    [Fact(DisplayName = "GetPresignedUrlAsync should return valid URL and filename")]
    public async Task GetPresignedUrlAsync_ShouldReturnValidUrlAndFilename()
    {
        const string expectedUrl = "http://minio/bucket/test-file.txt";
        _storageServiceMock
            .Setup(s => s.GetPresignedPutUrlAsync(It.IsAny<string>()))
            .ReturnsAsync(expectedUrl);
        
        var result = await _controller.GetPresignedUrlAsync();
        
        result.Should().BeOfType<JsonResult>();
        var jsonResult = result as JsonResult;
        jsonResult.Should().NotBeNull();
        
        var value = jsonResult!.Value;
        value.Should().NotBeNull();
        
        var urlProperty = value!.GetType().GetProperty("url");
        var filenameProperty = value.GetType().GetProperty("filename");
        
        urlProperty.Should().NotBeNull();
        filenameProperty.Should().NotBeNull();
        
        var url = urlProperty!.GetValue(value)?.ToString();
        var filename = filenameProperty!.GetValue(value)?.ToString();
        
        url.Should().Be("http://minio/bucket/test-file.txt");
        filename.Should().NotBeNullOrEmpty();
        Guid.TryParse(filename!.Replace(".txt", ""), out _).Should().BeTrue();
    }

    [Fact(DisplayName = "GetPresignedUrlAsync should generate unique filename")]
    public async Task GetPresignedUrlAsync_ShouldGenerateUniqueFilename()
    {
        _storageServiceMock
            .Setup(s => s.GetPresignedPutUrlAsync(It.IsAny<string>()))
            .ReturnsAsync("http://minio/bucket/file.txt");
        
        var result1 = await _controller.GetPresignedUrlAsync();
        var result2 = await _controller.GetPresignedUrlAsync();
        
        result1.Should().BeOfType<JsonResult>();
        result2.Should().BeOfType<JsonResult>();
        
        var filename1 = GetFilenameFromResult(result1 as JsonResult);
        var filename2 = GetFilenameFromResult(result2 as JsonResult);
        
        filename1.Should().NotBe(filename2);
    }

    [Fact(DisplayName = "ImportTransactionsAsync should call storage service and importer service")]
    public async Task ImportTransactionsAsync_ShouldCallStorageServiceAndImporterService()
    {
        const string filename = "test-file.txt";
        var importRequest = new ImportRequest { FileName = filename };
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("test content"));
        var expectedResult = new ImportResult
        {
            TotalLines = 10,
            SuccessCount = 8,
            FailedCount = 2
        };

        _storageServiceMock
            .Setup(s => s.DownloadFileStreamAsync(filename, It.IsAny<Func<Stream, CancellationToken, Task<ImportResult?>>>()))
            .Returns<string, Func<Stream, CancellationToken, Task<ImportResult?>>>(async (fileName, callback) => 
            {
                await callback(stream, CancellationToken.None);
            });

        _importerServiceMock
            .Setup(s => s.ImportFromStreamAsync(It.IsAny<Stream>(), filename))
            .ReturnsAsync(expectedResult);
        
        var result = await _controller.ImportTransactionsAsync(importRequest);
        
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedResult);
        
        _storageServiceMock.Verify(s => s.DownloadFileStreamAsync(filename, It.IsAny<Func<Stream, CancellationToken, Task<ImportResult?>>>()), Times.Once);
        _importerServiceMock.Verify(s => s.ImportFromStreamAsync(It.IsAny<Stream>(), filename), Times.Once);
    }

    [Fact(DisplayName = "ImportTransactionsAsync should return Ok with ImportResult")]
    public async Task ImportTransactionsAsync_ShouldReturnOkWithImportResult()
    {
        const string filename = "test-file.txt";
        var importRequest = new ImportRequest { FileName = filename };
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("test content"));
        var expectedResult = new ImportResult
        {
            TotalLines = 5,
            SuccessCount = 5,
            FailedCount = 0
        };

        _storageServiceMock
            .Setup(s => s.DownloadFileStreamAsync(filename, It.IsAny<Func<Stream, CancellationToken, Task<ImportResult?>>>()))
            .Returns<string, Func<Stream, CancellationToken, Task<ImportResult?>>>(async (fileName, callback) => 
            {
                await callback(stream, CancellationToken.None);
            });

        _importerServiceMock
            .Setup(s => s.ImportFromStreamAsync(It.IsAny<Stream>(), filename))
            .ReturnsAsync(expectedResult);
        
        var result = await _controller.ImportTransactionsAsync(importRequest);
        
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(expectedResult);
    }

    private static string? GetFilenameFromResult(JsonResult? result)
    {
        var filenameProperty = result?.Value?.GetType().GetProperty("filename");
        return filenameProperty?.GetValue(result?.Value)?.ToString();
    }
}

