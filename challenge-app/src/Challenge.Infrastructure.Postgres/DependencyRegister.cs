using Challenge.Domain.Contracts.Repository;
using Challenge.Infrastructure.Postgres.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Challenge.Infrastructure.Postgres;

public static class DependencyRegister
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services.AddScoped<ITransactionRepository, TransactionRepository>();
    }
}