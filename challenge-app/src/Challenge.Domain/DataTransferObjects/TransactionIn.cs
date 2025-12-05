namespace Challenge.Domain.DataTransferObjects;

public struct TransactionIn
{
    public string Type  { get; init; }
    public string Date { get; init; }
    public string Value { get; init; }
    public string TaxId { get; init; }
    public string Card { get; init; } 
    public string Time { get; init; }
    public string StoreOwner { get; init; }
    public string StoreName { get; init; }
}