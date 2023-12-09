using FastEndpoints;

namespace ApiBestPracticesExample.Presentation;

public class ErrorLogger : IGlobalPostProcessor
{
	public Task PostProcessAsync(IPostProcessorContext context, CancellationToken ct)
	{
		if (context.HasValidationFailures)
		{
			var logger = context.HttpContext.Resolve<ILogger>();
			logger.Warning("Validation error count: {Count}", context.ValidationFailures.Count);
		}
		return Task.CompletedTask;
	}
}