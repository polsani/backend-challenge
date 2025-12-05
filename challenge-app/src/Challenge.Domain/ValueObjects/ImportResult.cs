namespace Challenge.Domain.ValueObjects;

public class ImportResult
{
    public int TotalLines { get; set; }
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
}