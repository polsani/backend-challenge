using System.Text;
using Challenge.Infrastructure.Minio.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.DataModel.Response;
using Moq;

namespace Challenge.Infrastructure.Tests.Services;

public class MinioStorageServiceTests
{
    private readonly Mock<IMinioClient> _minioClientMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<ILogger<MinioStorageService>> _loggerMock;

    public MinioStorageServiceTests()
    {
        _minioClientMock = new Mock<IMinioClient>();
        _configurationMock = new Mock<IConfiguration>();
        _loggerMock = new Mock<ILogger<MinioStorageService>>();

        var bucketSection = new Mock<IConfigurationSection>();
        bucketSection.Setup(s => s.Value).Returns("transactions");

        var failedBucketSection = new Mock<IConfigurationSection>();
        failedBucketSection.Setup(s => s.Value).Returns("failed-transactions");

        _configurationMock
            .Setup(c => c.GetSection("Minio:BucketName"))
            .Returns(bucketSection.Object);

        _configurationMock
            .Setup(c => c.GetSection("Minio:FailedBucketName"))
            .Returns(failedBucketSection.Object);

        _minioClientMock
            .Setup(c => c.BucketExistsAsync(It.IsAny<BucketExistsArgs>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
    }

    [Fact(DisplayName = "Should delete file")]
    public async Task ShouldDeleteFile()
    {
        var service = CreateService();
        const string fileName = "test.txt";

        _minioClientMock
            .Setup(c => c.RemoveObjectAsync(It.IsAny<RemoveObjectArgs>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        await service.DeleteFileAsync(fileName);
        
        _minioClientMock.Verify(
            c => c.RemoveObjectAsync(
                It.IsAny<RemoveObjectArgs>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact(DisplayName = "Should get presigned get URL")]
    public async Task ShouldGetPresignedGetUrl()
    {
        var service = CreateService();
        const string fileName = "test.txt";
        const string expectedUrl = "http://minio:9000/transactions/test.txt?X-Amz-Algorithm=...";

        _minioClientMock
            .Setup(c => c.PresignedGetObjectAsync(It.IsAny<PresignedGetObjectArgs>()))
            .ReturnsAsync(expectedUrl);
        
        var result = await service.GetPresignedGetUrlAsync(fileName);
        
        result.Should().Be(expectedUrl);
        _minioClientMock.Verify(
            c => c.PresignedGetObjectAsync(
                It.IsAny<PresignedGetObjectArgs>()),
            Times.Once);
    }

    [Fact(DisplayName = "Should get presigned put URL")]
    public async Task ShouldGetPresignedPutUrl()
    {
        var service = CreateService();
        const string fileName = "test.txt";
        const string expectedUrl = "http://minio:9000/transactions/test.txt?X-Amz-Algorithm=...";

        _minioClientMock
            .Setup(c => c.PresignedPutObjectAsync(It.IsAny<PresignedPutObjectArgs>()))
            .ReturnsAsync(expectedUrl);
        
        var result = await service.GetPresignedPutUrlAsync(fileName);
        
        result.Should().Be(expectedUrl);
        _minioClientMock.Verify(
            c => c.PresignedPutObjectAsync(
                It.IsAny<PresignedPutObjectArgs>()),
            Times.Once);
    }

    [Fact(DisplayName = "Should respect expiry parameter for presigned URLs")]
    public async Task ShouldRespectExpiryParameterForPresignedUrls()
    {
        var service = CreateService();
        const string fileName = "test.txt";
        const int expiry = 7200;

        _minioClientMock
            .Setup(c => c.PresignedGetObjectAsync(It.IsAny<PresignedGetObjectArgs>()))
            .ReturnsAsync("http://minio/test.txt");
        
        await service.GetPresignedGetUrlAsync(fileName, expiry);
        
        _minioClientMock.Verify(
            c => c.PresignedGetObjectAsync(
                It.IsAny<PresignedGetObjectArgs>()),
            Times.Once);
    }

    [Fact(DisplayName = "Should create bucket if it does not exist")]
    public void ShouldCreateBucketIfItDoesNotExist()
    {
        _minioClientMock
            .Setup(c => c.BucketExistsAsync(It.IsAny<BucketExistsArgs>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _minioClientMock
            .Setup(c => c.MakeBucketAsync(It.IsAny<MakeBucketArgs>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        CreateService();
        
        _minioClientMock.Verify(
            c => c.MakeBucketAsync(
                It.IsAny<MakeBucketArgs>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact(DisplayName = "Should throw exception if bucket name not configured")]
    public void ShouldThrowExceptionIfBucketNameNotConfigured()
    {
        var emptySection = new Mock<IConfigurationSection>();
        emptySection.Setup(s => s.Value).Returns((string?)null);

        _configurationMock
            .Setup(c => c.GetSection("Minio:BucketName"))
            .Returns(emptySection.Object);
        
        var act = CreateService;
        act.Should().Throw<Exception>().WithMessage("*Bucket name not set*");
    }

    [Fact(DisplayName = "Should throw exception if failed bucket name not configured")]
    public void ShouldThrowExceptionIfFailedBucketNameNotConfigured()
    {
        var emptySection = new Mock<IConfigurationSection>();
        emptySection.Setup(s => s.Value).Returns((string?)null);

        _configurationMock
            .Setup(c => c.GetSection("Minio:FailedBucketName"))
            .Returns(emptySection.Object);
        
        var act = CreateService;
        act.Should().Throw<Exception>().WithMessage("*Failed bucket name not set*");
    }

    private MinioStorageService CreateService()
    {
        return new MinioStorageService(
            _minioClientMock.Object,
            _configurationMock.Object,
            _loggerMock.Object);
    }
}

