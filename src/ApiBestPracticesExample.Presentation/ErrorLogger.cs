using FastEndpoints;
using FluentValidation.Results;

namespace ApiBestPracticesExample.Presentation;

public class ErrorLogger : IGlobalPostProcessor
{
	public Task PostProcessAsync(object req, object? res, HttpContext ctx,
		IReadOnlyCollection<ValidationFailure> failures, CancellationToken ct)
	{
		if (failures.Count > 0)
		{
			var logger = ctx.Resolve<ILogger>();
			logger.Warning("Validation error count: {Count}", failures.Count);
		}

		return Task.CompletedTask;
	}
}