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
			var logger = ctx.Resolve<ILogger<ErrorLogger>>();
			logger.LogWarning("Validation error count: {@count}", failures.Count);
		}

		return Task.CompletedTask;
	}
}