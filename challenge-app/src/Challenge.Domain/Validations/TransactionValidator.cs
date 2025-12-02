namespace Challenge.Domain.Validations;

using Entities;
using FluentValidation;

internal sealed class TransactionValidator : AbstractValidator<Transaction>
{
    public TransactionValidator()
    {
        RuleFor(x=>x.Value).GreaterThan(0).WithMessage("Amount must be greater than 0");
        
    }
}