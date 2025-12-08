using Challenge.Domain.Contracts.Storage;
using Challenge.Infrastructure.Minio.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;

namespace Challenge.Infrastructure.Minio;

public static class DependencyRegister
{
    public static IServiceCollection AddStorage(this IServiceCollection services, IConfigurationManager configuration)
    {
        var minioUrl = configuration.GetSection("Minio:InternalUrl").Value;
        var minioAccessKey = configuration.GetSection("Minio:AccessKey").Value;
        var minioSecretKey = configuration.GetSection("Minio:SecretKey").Value;
        
        if(string.IsNullOrEmpty(minioUrl) || 
           string.IsNullOrEmpty(minioAccessKey) || 
           string.IsNullOrEmpty(minioSecretKey))
            throw new Exception("Missing configuration for Minio");
        
        return services
            .AddMinio(config => 
                config.WithEndpoint(minioUrl).WithCredentials(minioAccessKey, minioSecretKey).WithSSL(false).Build())
            .AddScoped<IStorageService, MinioStorageService>();
    }
}