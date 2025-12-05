using Challenge.Domain.Entities;
using Challenge.Domain.ValueObjects;
using FluentAssertions;

namespace Challenge.Domain.Tests.Core;

public class TransactionTypeTests
{
    [Theory]
    [InlineData(1, "Debit", OperationNature.Income, OperationSign.Plus)]
    [InlineData(2, "Boleto", OperationNature.Expense, OperationSign.Minus)]
    [InlineData(3, "Financing", OperationNature.Expense, OperationSign.Minus)]
    [InlineData(4, "Credit", OperationNature.Income, OperationSign.Plus)]
    [InlineData(5, "Loan Receipt", OperationNature.Income, OperationSign.Plus)]
    [InlineData(6, "Sales", OperationNature.Income, OperationSign.Plus)]
    [InlineData(7, "TED Receipt", OperationNature.Income, OperationSign.Plus)]
    [InlineData(8, "DOC Receipt", OperationNature.Income, OperationSign.Plus)]
    [InlineData(9, "Rent", OperationNature.Expense, OperationSign.Minus)]
    public void ShouldGetTransactionTypeWithCorrectProperties(ushort type, string description, 
        OperationNature nature, OperationSign sign)
    {
        var transactionType = TransactionType.Get(type);
        
        transactionType.Type.Should().Be(type);
        transactionType.Description.Should().Be(description);
        transactionType.Nature.Should().Be(nature);
        transactionType.Sign.Should().Be(sign);
    }

    [Fact(DisplayName = "Should throw exception when transaction type is invalid")]
    public void ShouldThrowExceptionWhenTransactionTypeIsInvalid()
    {
        var act = () => TransactionType.Get(99);
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact(DisplayName = "Should throw exception when transaction type is 0")]
    public void ShouldThrowExceptionWhenTransactionTypeIsZero()
    {
        var act = () => TransactionType.Get(0);
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact(DisplayName = "All income transaction types should have Plus sign")]
    public void AllIncomeTransactionTypesShouldHavePlusSign()
    {
        var incomeTypes = new[] { 1, 4, 5, 6, 7, 8 };
        
        foreach (var type in incomeTypes)
        {
            var transactionType = TransactionType.Get((ushort)type);
            transactionType.Nature.Should().Be(OperationNature.Income);
            transactionType.Sign.Should().Be(OperationSign.Plus);
        }
    }

    [Fact(DisplayName = "All expense transaction types should have Minus sign")]
    public void AllExpenseTransactionTypesShouldHaveMinusSign()
    {
        var expenseTypes = new[] { 2, 3, 9 };
        
        foreach (var type in expenseTypes)
        {
            var transactionType = TransactionType.Get((ushort)type);
            transactionType.Nature.Should().Be(OperationNature.Expense);
            transactionType.Sign.Should().Be(OperationSign.Minus);
        }
    }

    [Fact(DisplayName = "Should return same instance for same type")]
    public void ShouldReturnSameInstanceForSameType()
    {
        var type1 = TransactionType.Get(1);
        var type2 = TransactionType.Get(1);
        
        type1.Should().BeEquivalentTo(type2);
    }
}

