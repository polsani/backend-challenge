using Challenge.Domain.Contracts.Repository;
using Challenge.Domain.Entities;
using EFCore.BulkExtensions;

namespace Challenge.Infrastructure.Postgres.Repository;

public class TransactionRepository : ITransactionRepository
{
    private const int BatchSize = 1000;
    private readonly DefaultDbContext _dbContext = new("");
    private readonly BulkConfig _bulkConfig = new()
    {
        BatchSize = BatchSize,
        SetOutputIdentity = false,
        BulkCopyTimeout = 0
    };

    public async Task ImportTransactions(IEnumerable<Transaction> transactions)
    {
        await _dbContext.BulkInsertAsync(transactions, _bulkConfig);
    }
}
