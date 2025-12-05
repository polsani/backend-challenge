using Challenge.Domain.ValueObjects;

namespace Challenge.Domain.Contracts.Services;

public interface IImporterService
{
    Task<ImportResult> ImportFromStreamAsync(Stream stream, string fileName);
}