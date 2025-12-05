using Challenge.Domain.DataTransferObjects;
using Challenge.Domain.Entities;
using Challenge.Domain.ValueObjects;

namespace Challenge.Domain.Tests.Stubs;

public static class TransactionsStubs
{
    public static readonly TransactionIn ValidTransactionIn = new()
    {
        Type = "1",
        Value = "00067882",
        Card = "3424********8752",
        StoreOwner = "Owner",
        StoreName = "Store Name",
        TaxId = "28584565345",
        Time = "125000",
        Date = "20200428"
    };
    
    public static readonly TransactionIn InvalidTransactionIn = new()
    {
        Type = "1",
        Value = "00067882",
        Card = "3424********8752",
        StoreOwner = "Owner",
        StoreName = "Store Name",
        TaxId = "28584565645",
        Time = "125000",
        Date = "20200428"
    };

    public static readonly Entities.Transaction ValidTransactionEntity = new()
    {
        Type = TransactionType.Get(1),
        Value = 678.82m,
        StoreOwner = "Owner",
        StoreName = "Store Name",
        Card = "3424********8752",
        TaxId = new TaxId("28584565345"),
        Date = DateOnly.Parse("2020-04-28"),
        Time = TimeOnly.Parse("12:50:00")
    };
}