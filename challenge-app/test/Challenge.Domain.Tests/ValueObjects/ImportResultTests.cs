using Challenge.Domain.ValueObjects;
using FluentAssertions;

namespace Challenge.Domain.Tests.ValueObjects;

public class ImportResultTests
{
    [Fact(DisplayName = "Should initialize with zero counts")]
    public void ShouldInitializeWithZeroCounts()
    {
        // Arrange & Act
        var result = new ImportResult();

        // Assert
        result.TotalLines.Should().Be(0);
        result.SuccessCount.Should().Be(0);
        result.FailedCount.Should().Be(0);
    }

    [Fact(DisplayName = "Should allow setting TotalLines")]
    public void ShouldAllowSettingTotalLines()
    {
        // Arrange
        var result = new ImportResult();

        // Act
        result.TotalLines = 100;

        // Assert
        result.TotalLines.Should().Be(100);
    }

    [Fact(DisplayName = "Should allow setting SuccessCount")]
    public void ShouldAllowSettingSuccessCount()
    {
        // Arrange
        var result = new ImportResult();

        // Act
        result.SuccessCount = 80;

        // Assert
        result.SuccessCount.Should().Be(80);
    }

    [Fact(DisplayName = "Should allow setting FailedCount")]
    public void ShouldAllowSettingFailedCount()
    {
        // Arrange
        var result = new ImportResult();

        // Act
        result.FailedCount = 20;

        // Assert
        result.FailedCount.Should().Be(20);
    }

    [Fact(DisplayName = "SuccessCount and FailedCount should be less than or equal to TotalLines")]
    public void SuccessCountAndFailedCountShouldBeLessThanOrEqualToTotalLines()
    {
        // Arrange
        var result = new ImportResult
        {
            TotalLines = 100,
            SuccessCount = 80,
            FailedCount = 20
        };

        // Act & Assert
        (result.SuccessCount + result.FailedCount).Should().BeLessThanOrEqualTo(result.TotalLines);
    }

    [Fact(DisplayName = "Should handle case where SuccessCount + FailedCount equals TotalLines")]
    public void ShouldHandleCaseWhereCountsEqualTotalLines()
    {
        // Arrange
        var result = new ImportResult
        {
            TotalLines = 100,
            SuccessCount = 70,
            FailedCount = 30
        };

        // Act & Assert
        (result.SuccessCount + result.FailedCount).Should().Be(result.TotalLines);
    }

    [Fact(DisplayName = "Should handle case where SuccessCount + FailedCount is less than TotalLines")]
    public void ShouldHandleCaseWhereCountsLessThanTotalLines()
    {
        // Arrange
        var result = new ImportResult
        {
            TotalLines = 100,
            SuccessCount = 50,
            FailedCount = 30
        };

        // Act & Assert
        (result.SuccessCount + result.FailedCount).Should().BeLessThan(result.TotalLines);
    }
}

