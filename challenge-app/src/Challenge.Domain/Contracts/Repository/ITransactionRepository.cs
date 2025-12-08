using Challenge.Domain.Entities;

namespace Challenge.Domain.Contracts.Repository;

public interface ITransactionRepository
{
    Task ImportTransactionsAsync(IEnumerable<Transaction> transactions);
}