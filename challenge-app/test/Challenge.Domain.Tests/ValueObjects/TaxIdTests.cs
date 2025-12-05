using Challenge.Domain.ValueObjects;
using FluentAssertions;

namespace Challenge.Domain.Tests.ValueObjects;

public class TaxIdTests
{
    [Fact(DisplayName = "Should format TaxId correctly")]
    public void ShouldFormatTaxIdCorrectly()
    {
        var taxId = new TaxId("28584565345");
        var formatted = taxId.FormattedTaxId;
        
        formatted.Should().Be("285.845.653-45");
    }

    [Fact(DisplayName = "Should return raw value")]
    public void ShouldReturnRawValue()
    {
        const string rawTaxId = "28584565345";
        var taxId = new TaxId(rawTaxId);
        var value = taxId.Value;
        
        value.Should().Be(rawTaxId);
    }

    [Fact(DisplayName = "ToString should return formatted TaxId")]
    public void ToStringShouldReturnFormattedTaxId()
    {
        var taxId = new TaxId("28584565345");
        var result = taxId.ToString();
        
        result.Should().Be("285.845.653-45");
    }

    [Fact(DisplayName = "Should format TaxId with leading zeros")]
    public void ShouldFormatTaxIdWithLeadingZeros()
    {
        var taxId = new TaxId("01234567890");
        var formatted = taxId.FormattedTaxId;
        
        formatted.Should().Be("012.345.678-90");
    }

    [Theory]
    [InlineData("09620676017", "096.206.760-17")]
    [InlineData("11144477735", "111.444.777-35")]
    [InlineData("12345678909", "123.456.789-09")]
    public void ShouldFormatVariousTaxIds(string input, string expected)
    {
        var taxId = new TaxId(input);
        var formatted = taxId.FormattedTaxId;
        
        formatted.Should().Be(expected);
    }
}

