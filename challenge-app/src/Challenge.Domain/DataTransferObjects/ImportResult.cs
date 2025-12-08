using System.Text.Json.Serialization;
using Challenge.Domain.ValueObjects;

namespace Challenge.Domain.DataTransferObjects;

public class ImportResult
{
    public long TotalLines { get; set; }
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, SummaryResult>? Summary { get; set; }
    
    public void ClearSummary()
    {
        if (Summary != null)
        {
            foreach (var summary in Summary.Values)
            {
                summary.Clear();
            }
            Summary.Clear();
            Summary = null;
        }
    }
}