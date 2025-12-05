using Challenge.Domain.Validations;
using Challenge.Domain.ValueObjects;
using FluentAssertions;

namespace Challenge.Domain.Tests.Validations;

public class TaxIdValidatorTests
{
    private readonly TaxIdValidator _validator = new();

    [Fact(DisplayName = "Should validate valid TaxId")]
    public void ShouldValidateValidTaxId()
    {
        var taxId = new TaxId("28584565345");
        var result = _validator.Validate(taxId);
        
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("09620676017")]
    [InlineData("11144477735")]
    [InlineData("12345678909")]
    public void ShouldValidateMultipleValidTaxIds(string strTaxId)
    {
        var taxId = new TaxId(strTaxId);
        var result = _validator.Validate(taxId);
        
        result.IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Should invalidate TaxIds with wrong check digits")]
    public void ShouldInvalidateTaxIdsWithWrongCheckDigits()
    {
        var invalidTaxId = new TaxId("28584565645");
        var result = _validator.Validate(invalidTaxId);
        
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Invalid TaxId.");
    }

    [Fact(DisplayName = "Should invalidate TaxId with less than 11 digits")]
    public void ShouldInvalidateTaxIdWithLessThan11Digits()
    {
        var invalidTaxId = new TaxId("1234567890");
        var result = _validator.Validate(invalidTaxId);
        
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "TaxId must be 11 digits long.");
    }

    [Fact(DisplayName = "Should invalidate TaxId with more than 11 digits")]
    public void ShouldInvalidateTaxIdWithMoreThan11Digits()
    {
        var invalidTaxId = new TaxId("123456789012");
        var result = _validator.Validate(invalidTaxId);
        
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "TaxId must be 11 digits long.");
    }
}

