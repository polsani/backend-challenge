using Challenge.Domain.DataTransferObjects;
using Challenge.Domain.ValueObjects;

namespace Challenge.Domain.Contracts.Storage;

public interface IStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);
    Task UploadFailedFileAsync(Stream fileStream, string fileName, string contentType);

    Task DownloadFileStreamAsync(string fileName, Func<Stream, CancellationToken, Task<ImportResult?>> callback);
    Task DeleteFileAsync(string fileName);
    Task<string> GetPresignedGetUrlAsync(string fileName, int expiryInSeconds = 3600);
    Task<string> GetPresignedPutUrlAsync(string fileName, int expiryInSeconds = 3600);
}