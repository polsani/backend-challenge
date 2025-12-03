using Challenge.Domain.Contracts.Storage;
using Challenge.Infrastructure.Minio.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Challenge.Infrastructure.Minio;

public static class DependencyRegister
{
    public static IServiceCollection AddStorage(this IServiceCollection services)
    {
        return services.AddScoped<IStorageService, MinioStorageService>();
    }
}