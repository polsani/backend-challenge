using Challenge.Domain.DataTransferObjects;
using Challenge.Domain.Entities;
using Challenge.Domain.Tests.Stubs;
using Challenge.Domain.ValueObjects;
using Mapster;
using Transaction = Challenge.Domain.Adapters.Transaction;

namespace Challenge.Domain.Tests.Adapters;

public class TransactionTests
{
    public TransactionTests()
    {
        Transaction.Configure();    
    }

    [Fact(DisplayName = "Should be able to adapt from TransactionIn to Transaction entity")]
    public void ShouldBeAbleToAdaptFromTransactionInToTransactionEntity()
    {
        var transactionEntity = TransactionsStubs.ValidTransactionIn.Adapt<Entities.Transaction>();
        
        Assert.Equivalent(transactionEntity, new Entities.Transaction
        {
            Card = "3424********8752",
            StoreOwner = "Owner",
            StoreName = "Store Name",
            Date = DateOnly.Parse("2020-04-28"),
            Time = TimeOnly.Parse("12:50:00"),
            Type =  TransactionType.Get(1),
            Value = 678.82m,
            TaxId = new TaxId("28584565345")
        });
    }
    
    [Fact(DisplayName = "Should throw an exception when TransactionIn is invalid")]
    public void  ShouldThrowAnExceptionWhenTransactionInIsInvalid()
    {
        Assert.Throws<ArgumentException>(() => TransactionsStubs.ValidTransactionIn.Adapt<Entities.Transaction>());
    }

    [Fact(DisplayName = "Should generate Id")]
    public void ShouldGenerateId()
    {
        var transactionEntity = TransactionsStubs.ValidTransactionIn.Adapt<Entities.Transaction>();
        
        Assert.NotEqual(0, transactionEntity.Id);
    }
    
    [Fact(DisplayName = "Should be able to adapt from Transaction entity to TransactionOut")]
    public void ShouldBeAbleToAdaptFromTransactionEntityToTransactionOut()
    {
        var transactionOut = TransactionsStubs.ValidTransactionEntity.Adapt<TransactionOut>();
        
        Assert.Equivalent(transactionOut, new TransactionOut
        {
            Card = "3424********8752",
            StoreOwner = "Owner",
            StoreName = "Store Name",
            Date = "2020-04-28",
            Time = "12:50:00",
            Type = TransactionType.Get(1).Description,
            Value = 678.82m,
            TaxId = "285.845.653-45"
        });
    }
}