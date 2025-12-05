namespace Challenge.Domain.Validations;

using ValueObjects;
using FluentValidation;

public sealed class TaxIdValidator : AbstractValidator<TaxId>
{
    public TaxIdValidator()
    {
        RuleFor(taxId => taxId.Value)
            .NotEmpty().WithMessage("TaxId is required.")
            .Length(11).WithMessage("TaxId must be 11 digits long.")
            .Must(VerifyDigit).WithMessage("Invalid TaxId.");
    }
    
    private static bool VerifyDigit(string taxId)
    {
        int[] multiplier1 = [10, 9, 8, 7, 6, 5, 4, 3, 2];
        int[] multiplier2 = [11, 10, 9, 8, 7, 6, 5, 4, 3, 2];

        var tmpTaxId = taxId[..9];
        var sum = 0;

        for (var i = 0; i < 9; i++)
            sum += int.Parse(tmpTaxId[i].ToString()) * multiplier1[i];

        var modResult = sum % 11;
        modResult = modResult < 2 ? 0 : 11 - modResult;

        var digit = modResult.ToString();
        tmpTaxId += digit;
        sum = 0;

        for (var i = 0; i < 10; i++)
            sum += int.Parse(tmpTaxId[i].ToString()) * multiplier2[i];

        modResult = sum % 11;
        modResult = modResult < 2 ? 0 : 11 - modResult;
        digit += modResult.ToString();

        return taxId.EndsWith(digit);
    }
}