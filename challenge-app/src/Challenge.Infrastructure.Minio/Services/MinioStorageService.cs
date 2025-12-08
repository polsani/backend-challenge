using Challenge.Domain.Contracts.Storage;
using Challenge.Domain.DataTransferObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;

namespace Challenge.Infrastructure.Minio.Services;

public class MinioStorageService : IStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly ILogger<MinioStorageService> _logger;
    private readonly string? _bucketName;
    private readonly string? _failedBucketName;

    public MinioStorageService(IMinioClient minioClient, IConfiguration configuration, 
        ILogger<MinioStorageService> logger)
    {
        _minioClient = minioClient;
        _logger = logger;
        _bucketName = configuration.GetSection("Minio:BucketName").Value;
        _failedBucketName = configuration.GetSection("Minio:FailedBucketName").Value;
        
        if(string.IsNullOrEmpty(_bucketName))
            throw new Exception("Minio: Bucket name not set");
        
        if(string.IsNullOrEmpty(_failedBucketName))
            throw new Exception("Minio: Failed bucket name not set");
        
        EnsureBucketExistsAsync().GetAwaiter().GetResult();
    }
    
    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
    {
        var objectName = $"{Guid.NewGuid()}_{fileName}";
            
        await _minioClient.PutObjectAsync(new PutObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectName)
            .WithStreamData(fileStream)
            .WithObjectSize(fileStream.Length)
            .WithContentType(contentType));

        return objectName;
    }

    public async Task UploadFailedFileAsync(Stream fileStream, string fileName, string contentType)
    {
        _logger.LogWarning("Invalid transactions persisted into {bucket} in file {fileName}", _failedBucketName, fileName);
        
        await _minioClient.PutObjectAsync(new PutObjectArgs()
            .WithBucket(_failedBucketName)
            .WithObject(fileName)
            .WithStreamData(fileStream)
            .WithObjectSize(fileStream.Length)
            .WithContentType(contentType));
    }

    public async Task DownloadFileStreamAsync(string fileName, Func<Stream, CancellationToken, Task<ImportResult?>> callback)
    {
        await _minioClient.GetObjectAsync(new GetObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(fileName)
            .WithCallbackStream(callback));
    }

    public async Task DeleteFileAsync(string fileName)
    {
        await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(fileName));
    }

    public async Task<string> GetPresignedGetUrlAsync(string fileName, int expiryInSeconds = 3600)
    {
        return await _minioClient.PresignedGetObjectAsync(new PresignedGetObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(fileName)
            .WithExpiry(expiryInSeconds));
    }
    
    public async Task<string> GetPresignedPutUrlAsync(string fileName, int expiryInSeconds = 3600)
    {
        return await _minioClient.PresignedPutObjectAsync(new PresignedPutObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(fileName)
            .WithExpiry(expiryInSeconds));
    }
    
    private async Task EnsureBucketExistsAsync()
    {
        var bucketExists = await _minioClient.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(_bucketName));
            
        if (!bucketExists)
        {
            await _minioClient.MakeBucketAsync(
                new MakeBucketArgs().WithBucket(_bucketName));
        }
    }
}