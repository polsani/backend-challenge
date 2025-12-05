using Challenge.Domain.Entities;
using Challenge.Domain.Validations;
using Challenge.Domain.ValueObjects;
using FluentAssertions;

namespace Challenge.Domain.Tests.Validations;

public class TransactionValidatorTests
{
    private readonly TransactionValidator _validator = new();

    [Fact(DisplayName = "Should validate transaction with value greater than 0")]
    public void ShouldValidateTransactionWithValueGreaterThanZero()
    {
        var transaction = new Transaction
        {
            Type = TransactionType.Get(1),
            Date = DateOnly.Parse("2020-04-28"),
            Value = 100.50m,
            TaxId = new TaxId("28584565345"),
            Card = "1234****5678",
            Time = TimeOnly.Parse("12:00:00"),
            StoreOwner = "Owner",
            StoreName = "Store"
        };
        
        var result = _validator.Validate(transaction);
        
        result.IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Should invalidate transaction with value equal to 0")]
    public void ShouldInvalidateTransactionWithValueEqualToZero()
    {
        var transaction = new Transaction
        {
            Type = TransactionType.Get(1),
            Date = DateOnly.Parse("2020-04-28"),
            Value = 0m,
            TaxId = new TaxId("28584565345"),
            Card = "1234****5678",
            Time = TimeOnly.Parse("12:00:00"),
            StoreOwner = "Owner",
            StoreName = "Store"
        };
        
        var result = _validator.Validate(transaction);
        
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Amount must be greater than 0");
    }

    [Fact(DisplayName = "Should invalidate transaction with negative value")]
    public void ShouldInvalidateTransactionWithNegativeValue()
    {
        var transaction = new Transaction
        {
            Type = TransactionType.Get(1),
            Date = DateOnly.Parse("2020-04-28"),
            Value = -100.50m,
            TaxId = new TaxId("28584565345"),
            Card = "1234****5678",
            Time = TimeOnly.Parse("12:00:00"),
            StoreOwner = "Owner",
            StoreName = "Store"
        };
        
        var result = _validator.Validate(transaction);
        
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Amount must be greater than 0");
    }

    [Fact(DisplayName = "Should validate transaction with very large value")]
    public void ShouldValidateTransactionWithVeryLargeValue()
    {
        var transaction = new Transaction
        {
            Type = TransactionType.Get(1),
            Date = DateOnly.Parse("2020-04-28"),
            Value = 999999999.99m,
            TaxId = new TaxId("28584565345"),
            Card = "1234****5678",
            Time = TimeOnly.Parse("12:00:00"),
            StoreOwner = "Owner",
            StoreName = "Store"
        };
        
        var result = _validator.Validate(transaction);
        
        result.IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Should validate transaction with small decimal value")]
    public void ShouldValidateTransactionWithSmallDecimalValue()
    {
        var transaction = new Transaction
        {
            Type = TransactionType.Get(1),
            Date = DateOnly.Parse("2020-04-28"),
            Value = 0.01m,
            TaxId = new TaxId("28584565345"),
            Card = "1234****5678",
            Time = TimeOnly.Parse("12:00:00"),
            StoreOwner = "Owner",
            StoreName = "Store"
        };
        
        var result = _validator.Validate(transaction);
        
        result.IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Should validate transaction with decimal precision")]
    public void ShouldValidateTransactionWithDecimalPrecision()
    {
        var transaction = new Transaction
        {
            Type = TransactionType.Get(1),
            Date = DateOnly.Parse("2020-04-28"),
            Value = 123.456789m,
            TaxId = new TaxId("28584565345"),
            Card = "1234****5678",
            Time = TimeOnly.Parse("12:00:00"),
            StoreOwner = "Owner",
            StoreName = "Store"
        };
        
        var result = _validator.Validate(transaction);
        
        result.IsValid.Should().BeTrue();
    }
}

