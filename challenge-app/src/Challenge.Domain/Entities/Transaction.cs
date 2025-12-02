using Challenge.Domain.ValueObjects;

namespace Challenge.Domain.Entities;

public class Transaction
{
    public int Id { get; set; }
    public TransactionType Type { get; set; }
    public DateOnly Date { get; set; }
    public decimal Value { get; set; }
    public TaxId TaxId { get; set; }
    public string Card { get; set; }
    public TimeOnly Time { get; set; }
    public string StoreOwner { get; set; }
    public string StoreName { get; set; }
}