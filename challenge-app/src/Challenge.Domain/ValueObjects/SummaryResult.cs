using Challenge.Domain.Entities;

namespace Challenge.Domain.ValueObjects;

public class SummaryResult
{
    public IList<Transaction> Transactions { get; } = new List<Transaction>();
    public decimal Balance { get; set; }
    
    public void Clear()
    {
        Transactions.Clear();
    }
}