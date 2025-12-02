namespace Challenge.Domain.DataTransferObjects;

public struct TransactionIn
{
    public string Type  { get; set; }
    public string Date { get; set; }
    public string Value { get; set; }
    public string TaxId { get; set; }
    public string Card { get; set; } 
    public string Time { get; set; }
    public string StoreOwner { get; set; }
    public string StoreName { get; set; }
}