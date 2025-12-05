using Challenge.Domain.DataTransferObjects;
using Mapster;

namespace Challenge.Domain.Adapters;

public static class ImportResult
{
    public static void Configure()
    {
        TypeAdapterConfig<ValueObjects.ImportResult, ImportResultOut>
            .NewConfig()
            .Map(dest => dest.SuccessCount, src => src.SuccessCount)
            .Map(dest => dest.FailedCount, src => src.FailedCount)
            .Map(dest => dest.TotalLines, src => src.TotalLines);
        
        TypeAdapterConfig<ImportResultOut, ValueObjects.ImportResult>
            .NewConfig()
            .Map(dest => dest.SuccessCount, src => src.SuccessCount)
            .Map(dest => dest.FailedCount, src => src.FailedCount)
            .Map(dest => dest.TotalLines, src => src.TotalLines);
    }
}