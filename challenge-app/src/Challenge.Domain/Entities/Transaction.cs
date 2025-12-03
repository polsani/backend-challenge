using System.ComponentModel.DataAnnotations;
using Challenge.Domain.ValueObjects;

namespace Challenge.Domain.Entities;

public class Transaction
{
    [Key]
    public int Id { get; set; }
    public required TransactionType Type { get; init; }
    public required DateOnly Date { get; init; }
    public required decimal Value { get; init; }
    public required TaxId TaxId { get; init; }
    public required string Card { get; init; }
    public required TimeOnly Time { get; init; }
    public required string StoreOwner { get; init; }
    public required string StoreName { get; init; }
}