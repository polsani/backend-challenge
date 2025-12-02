namespace Challenge.Domain.Contracts.Storage;

public interface IStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);
    Task<Stream> DownloadFileAsync(string fileName);
    Task DeleteFileAsync(string fileName);
    Task<string> GetPresignedUrlAsync(string fileName, int expiryInSeconds = 3600);
}