using Challenge.Domain.Adapters;
using Challenge.Domain.Entities;
using Challenge.Domain.ValueObjects;
using FluentAssertions;

namespace Challenge.Domain.Tests.Core;

public class TransactionTests
{
    public TransactionTests()
    {
        AdapterModule.ConfigureAll();
    }

    [Fact(DisplayName = "Should create a new transaction from a string")]
    public void ShouldCreateANewTransactionFromAString()
    {
        const string rawTransaction = "3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO       ";
        var expectedTransaction = new Entities.Transaction
        {
            Type = TransactionType.Get(3),
            Date = DateOnly.Parse("2019-03-01"),
            Value = 142.00m,
            TaxId = new TaxId("09620676017"),
            Card = "4753****3153",
            Time = TimeOnly.Parse("15:34:53"),
            StoreOwner = "JOÃO MACEDO",
            StoreName = "BAR DO JOÃO"
        };
        
        var result = Entities.Transaction.FromString(rawTransaction);
        result.Should().BeEquivalentTo(expectedTransaction, options => options.Excluding(t => t.Id));
    }

    [Theory]
    [InlineData("1", "Debit")]
    [InlineData("2", "Boleto")]
    [InlineData("3", "Financing")]
    [InlineData("4", "Credit")]
    [InlineData("5", "Loan Receipt")]
    [InlineData("6", "Sales")]
    [InlineData("7", "TED Receipt")]
    [InlineData("8", "DOC Receipt")]
    [InlineData("9", "Rent")]
    public void ShouldCreateTransactionFromStringWithAllTransactionTypes(string type, string expectedDescription)
    {
        var rawTransaction = $"{type}202004280000678822858456534534242****8752125000OWNER NAME     STORE NAME       ";
        
        var transaction = Entities.Transaction.FromString(rawTransaction);
        
        transaction.Type.Description.Should().Be(expectedDescription);
    }

    [Fact(DisplayName = "Should throw exception when string is shorter than 81 characters")]
    public void ShouldThrowExceptionWhenStringIsShorterThan81Characters()
    {
        var shortString = "3201903010000014200096206760174753****3153153453JOÃO MACEDO";
        
        var act = () => Entities.Transaction.FromString(shortString);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact(DisplayName = "Should throw exception when string is empty")]
    public void ShouldThrowExceptionWhenStringIsEmpty()
    {
        var emptyString = "";
        
        var act = () => Entities.Transaction.FromString(emptyString);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact(DisplayName = "Should handle string with trailing spaces")]
    public void ShouldHandleStringWithTrailingSpaces()
    {
        var rawTransaction = "3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO       ";
        
        var transaction = Entities.Transaction.FromString(rawTransaction);
        
        transaction.StoreName.Should().Be("BAR DO JOÃO");
        transaction.StoreOwner.Should().Be("JOÃO MACEDO");
    }

    [Fact(DisplayName = "Should parse value with leading zeros correctly")]
    public void ShouldParseValueWithLeadingZerosCorrectly()
    {
        const string rawTransaction = "1202004280000000128584565345342424****8752125000OWNER NAME     STORE NAME       ";
        
        var transaction = Entities.Transaction.FromString(rawTransaction);
        
        transaction.Value.Should().Be(1.28m);
    }

    [Fact(DisplayName = "Should parse large value correctly")]
    public void ShouldParseLargeValueCorrectly()
    {
        const string rawTransaction = "1202004289999999999285845653453424****8752125000OWNER NAME     STORE NAME       ";
                                      
        var transaction = Entities.Transaction.FromString(rawTransaction);
        
        transaction.Value.Should().Be(99999999.99m);
    }

    [Fact(DisplayName = "Should parse date correctly")]
    public void ShouldParseDateCorrectly()
    {
        const string rawTransaction = "1202002280000000128584565345342424****8752125000OWNER NAME     STORE NAME       ";
        
        var transaction = Entities.Transaction.FromString(rawTransaction);
        
        transaction.Date.Should().Be(DateOnly.Parse("2020-02-28"));
    }

    [Fact(DisplayName = "Should parse time correctly")]
    public void ShouldParseTimeCorrectly()
    {
        const string rawTransaction = "1202004280000000128584565345342424****8752000000OWNER NAME     STORE NAME       ";
        
        var transaction = Entities.Transaction.FromString(rawTransaction);
        
        transaction.Time.Should().Be(TimeOnly.Parse("00:00:00"));
    }

    [Fact(DisplayName = "Should parse time at end of day")]
    public void ShouldParseTimeAtEndOfDay()
    {
        const string rawTransaction = "1202004280000000128584565345342424****8752235959OWNER NAME     STORE NAME       ";
        
        var transaction = Entities.Transaction.FromString(rawTransaction);
        
        transaction.Time.Should().Be(TimeOnly.Parse("23:59:59"));
    }

    [Fact(DisplayName = "Should throw exception when transaction type is invalid")]
    public void ShouldThrowExceptionWhenTransactionTypeIsInvalid()
    {
        const string rawTransaction = "0202004280000000128584565345342424****8752125000OWNER NAME     STORE NAME       ";
        
        var act = () => Entities.Transaction.FromString(rawTransaction);
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact(DisplayName = "Should handle special characters in store name and owner")]
    public void ShouldHandleSpecialCharactersInStoreNameAndOwner()
    {
        const string rawTransaction = "1202004280000000128584565345342424****8752125000JOSÉ & MARIA   CAFÉ DO JOÃO     ";
        
        var transaction = Entities.Transaction.FromString(rawTransaction);
        
        transaction.StoreOwner.Should().Be("JOSÉ & MARIA");
        transaction.StoreName.Should().Be("CAFÉ DO JOÃO");
    }

    [Fact(DisplayName = "Should trim whitespace from store name and owner")]
    public void ShouldTrimWhitespaceFromStoreNameAndOwner()
    {
        const string rawTransaction = "1202004280000000128584565345342424****8752125000OWNER NAME     STORE NAME       ";
        
        var transaction = Entities.Transaction.FromString(rawTransaction);
        
        transaction.StoreOwner.Should().Be("OWNER NAME");
        transaction.StoreName.Should().Be("STORE NAME");
    }
}