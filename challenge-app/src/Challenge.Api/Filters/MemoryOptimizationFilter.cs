using Challenge.Domain.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Challenge.Api.Filters;

public class MemoryOptimizationFilter : IResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context) {}

    public void OnResultExecuted(ResultExecutedContext context)
    {
        if (context.Result is ObjectResult { Value: ImportResult importResult })
        {
            importResult.ClearSummary();
        }
    }
}
