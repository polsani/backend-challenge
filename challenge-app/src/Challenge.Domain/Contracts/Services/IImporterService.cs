using Challenge.Domain.DataTransferObjects;

namespace Challenge.Domain.Contracts.Services;

public interface IImporterService
{
    Task<ImportResult> ImportFromStreamAsync(Stream stream, string fileName, bool showSummary = false);
}
