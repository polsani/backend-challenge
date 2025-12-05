using Challenge.Domain.Contracts.Services;
using Challenge.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Challenge.Domain;

public static class DependencyRegister
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        return services
            .AddScoped<IImporterService, ImporterService>();
    }
}