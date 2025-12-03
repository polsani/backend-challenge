using Challenge.Domain.Contracts.Repository;
using Challenge.Domain.Entities;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace Challenge.Infrastructure.Postgres.Repository;

public class TransactionRepository(DefaultDbContext dbContext) : ITransactionRepository
{
    private const int BatchSize = 1000;

    private readonly BulkConfig _bulkConfig = new()
    {
        BatchSize = BatchSize,
        SetOutputIdentity = false,
        BulkCopyTimeout = 0
    };

    public async Task ImportTransactions(IEnumerable<Transaction> transactions)
    {
        await dbContext.BulkInsertAsync(transactions, _bulkConfig);
    }

    public async Task<IEnumerable<Transaction>> GetTransactions(int pageSize = 0, int pageNumber = 0)
    {
        IQueryable<Transaction> query = dbContext.Transactions.OrderBy(x=>x.Id);

        if (pageSize != 0)
            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        
        return await query.ToListAsync();
    }
}
