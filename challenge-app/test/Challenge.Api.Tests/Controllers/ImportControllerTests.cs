using System.Text;
using Challenge.Api.Controllers.v1;
using Challenge.Domain.Contracts.Services;
using Challenge.Domain.Contracts.Storage;
using Challenge.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Challenge.Api.Tests.Controllers;

public class ImportControllerTests
{
    private readonly Mock<IStorageService> _storageServiceMock;
    private readonly Mock<IImporterService> _importerServiceMock;
    private readonly ImportController _controller;

    public ImportControllerTests()
    {
        _storageServiceMock = new Mock<IStorageService>();
        _importerServiceMock = new Mock<IImporterService>();
        _controller = new ImportController(_storageServiceMock.Object, _importerServiceMock.Object);
    }

    [Fact(DisplayName = "GetPresignedUrlAsync should return valid URL and filename")]
    public async Task GetPresignedUrlAsync_ShouldReturnValidUrlAndFilename()
    {
        // Arrange
        var expectedUrl = "http://minio/bucket/test-file.txt";
        _storageServiceMock
            .Setup(s => s.GetPresignedPutUrlAsync(It.IsAny<string>()))
            .ReturnsAsync(expectedUrl);

        // Act
        var result = await _controller.GetPresignedUrlAsync();

        // Assert
        result.Should().BeOfType<JsonResult>();
        var jsonResult = result as JsonResult;
        jsonResult.Should().NotBeNull();
        
        var value = jsonResult!.Value;
        value.Should().NotBeNull();
        
        // Use reflection to check properties
        var urlProperty = value!.GetType().GetProperty("url");
        var filenameProperty = value.GetType().GetProperty("filename");
        
        urlProperty.Should().NotBeNull();
        filenameProperty.Should().NotBeNull();
        
        var url = urlProperty!.GetValue(value)?.ToString();
        var filename = filenameProperty!.GetValue(value)?.ToString();
        
        url.Should().Be("http://localhost/bucket/test-file.txt");
        filename.Should().NotBeNullOrEmpty();
        Guid.TryParse(filename!.Replace(".txt", ""), out _).Should().BeTrue();
    }

    [Fact(DisplayName = "GetPresignedUrlAsync should replace http://minio with http://localhost")]
    public async Task GetPresignedUrlAsync_ShouldReplaceMinioWithLocalhost()
    {
        // Arrange
        var minioUrl = "http://minio:9000/bucket/filename.txt";
        _storageServiceMock
            .Setup(s => s.GetPresignedPutUrlAsync(It.IsAny<string>()))
            .ReturnsAsync(minioUrl);

        // Act
        var result = await _controller.GetPresignedUrlAsync();

        // Assert
        result.Should().BeOfType<JsonResult>();
        var jsonResult = result as JsonResult;
        var value = jsonResult!.Value;
        var urlProperty = value!.GetType().GetProperty("url");
        var url = urlProperty!.GetValue(value)?.ToString();
        
        url.Should().Be("http://localhost:9000/bucket/filename.txt");
        url.Should().NotContain("minio");
    }

    [Fact(DisplayName = "GetPresignedUrlAsync should generate unique filename")]
    public async Task GetPresignedUrlAsync_ShouldGenerateUniqueFilename()
    {
        // Arrange
        _storageServiceMock
            .Setup(s => s.GetPresignedPutUrlAsync(It.IsAny<string>()))
            .ReturnsAsync("http://minio/bucket/file.txt");

        // Act
        var result1 = await _controller.GetPresignedUrlAsync();
        var result2 = await _controller.GetPresignedUrlAsync();

        // Assert
        result1.Should().BeOfType<JsonResult>();
        result2.Should().BeOfType<JsonResult>();
        
        var filename1 = GetFilenameFromResult(result1 as JsonResult);
        var filename2 = GetFilenameFromResult(result2 as JsonResult);
        
        filename1.Should().NotBe(filename2);
    }

    [Fact(DisplayName = "ImportTransactionsAsync should call storage service and importer service")]
    public async Task ImportTransactionsAsync_ShouldCallStorageServiceAndImporterService()
    {
        // Arrange
        var filename = "test-file.txt";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("test content"));
        var expectedResult = new ImportResult
        {
            TotalLines = 10,
            SuccessCount = 8,
            FailedCount = 2
        };

        _storageServiceMock
            .Setup(s => s.DownloadFileAsync(filename))
            .ReturnsAsync(stream);

        _importerServiceMock
            .Setup(s => s.ImportFromStreamAsync(It.IsAny<Stream>(), filename))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.ImportTransactionsAsync(filename);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedResult);
        
        _storageServiceMock.Verify(s => s.DownloadFileAsync(filename), Times.Once);
        _importerServiceMock.Verify(s => s.ImportFromStreamAsync(It.IsAny<Stream>(), filename), Times.Once);
    }

    [Fact(DisplayName = "ImportTransactionsAsync should return Ok with ImportResult")]
    public async Task ImportTransactionsAsync_ShouldReturnOkWithImportResult()
    {
        // Arrange
        var filename = "test-file.txt";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("test content"));
        var expectedResult = new ImportResult
        {
            TotalLines = 5,
            SuccessCount = 5,
            FailedCount = 0
        };

        _storageServiceMock
            .Setup(s => s.DownloadFileAsync(filename))
            .ReturnsAsync(stream);

        _importerServiceMock
            .Setup(s => s.ImportFromStreamAsync(It.IsAny<Stream>(), filename))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.ImportTransactionsAsync(filename);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(expectedResult);
    }

    private static string? GetFilenameFromResult(JsonResult? result)
    {
        if (result?.Value == null) return null;
        
        var filenameProperty = result.Value.GetType().GetProperty("filename");
        return filenameProperty?.GetValue(result.Value)?.ToString();
    }
}

