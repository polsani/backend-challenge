using Challenge.Domain.DataTransferObjects;
using Challenge.Domain.ValueObjects;
using FluentAssertions;

namespace Challenge.Domain.Tests.ValueObjects;

public class ImportResultTests
{
    [Fact(DisplayName = "Should initialize with zero counts")]
    public void ShouldInitializeWithZeroCounts()
    {
        var result = new ImportResult();

        result.TotalLines.Should().Be(0);
        result.SuccessCount.Should().Be(0);
        result.FailedCount.Should().Be(0);
    }

    [Fact(DisplayName = "SuccessCount and FailedCount should be less than or equal to TotalLines")]
    public void SuccessCountAndFailedCountShouldBeLessThanOrEqualToTotalLines()
    {
        var result = new ImportResult
        {
            TotalLines = 100,
            SuccessCount = 80,
            FailedCount = 20
        };
        
        ((long)(result.SuccessCount + result.FailedCount)).Should().BeLessThanOrEqualTo(result.TotalLines);
    }

    [Fact(DisplayName = "Should handle case where SuccessCount + FailedCount equals TotalLines")]
    public void ShouldHandleCaseWhereCountsEqualTotalLines()
    {
        var result = new ImportResult
        {
            TotalLines = 100,
            SuccessCount = 70,
            FailedCount = 30
        };
        
        ((long)(result.SuccessCount + result.FailedCount)).Should().Be(result.TotalLines);
    }

    [Fact(DisplayName = "Should handle case where SuccessCount + FailedCount is less than TotalLines")]
    public void ShouldHandleCaseWhereCountsLessThanTotalLines()
    {
        var result = new ImportResult
        {
            TotalLines = 100,
            SuccessCount = 50,
            FailedCount = 30
        };
        
        ((long)(result.SuccessCount + result.FailedCount)).Should().BeLessThan(result.TotalLines);
    }
}

