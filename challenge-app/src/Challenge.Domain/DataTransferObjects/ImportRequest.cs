using System.Text.Json.Serialization;

namespace Challenge.Domain.DataTransferObjects;

public class ImportRequest
{
    [JsonPropertyName("filename")]
    public required string FileName { get; init; }

    [JsonPropertyName("showSummary")]
    public bool ShowSummary { get; set; } = false;
}