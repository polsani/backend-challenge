namespace Challenge.Domain.DataTransferObjects;

public class ImportResultOut
{
    public int TotalLines { get; set; }
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
}