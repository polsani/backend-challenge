using Challenge.Domain.Entities;

namespace Challenge.Domain.Contracts.Repository;

public interface ITransactionRepository
{
    Task ImportTransactions(IEnumerable<Transaction> transactions);
    Task<IEnumerable<Transaction>> GetTransactions(int pageSize = 0, int pageNumber = 0);
}