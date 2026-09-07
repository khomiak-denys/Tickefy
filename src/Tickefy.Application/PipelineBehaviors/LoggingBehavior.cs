using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using Tickefy.Domain.Common.Results;

namespace Tickefy.Application.PipelineBehaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;

            _logger.LogInformation("Handling {RequestName}", requestName);

            var stopwatch = Stopwatch.StartNew();

            try
            {
                var response = await next(cancellationToken);
                stopwatch.Stop();

                if (response is IResult { IsFailure: true } result)
                {
                    _logger.LogWarning(
                        "Handled {RequestName} with error: {ErrorMessage} ({ElapsedMilliseconds} ms)",
                        requestName,
                        result.Error?.Message,
                        stopwatch.ElapsedMilliseconds);
                }
                else
                {
                    _logger.LogInformation(
                        "Handled {RequestName} successfully ({ElapsedMilliseconds} ms)",
                        requestName,
                        stopwatch.ElapsedMilliseconds);
                }

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(
                    ex,
                    "Handled {RequestName} failed with an unhandled exception ({ElapsedMilliseconds} ms)",
                    requestName,
                    stopwatch.ElapsedMilliseconds);

                throw;
            }
        }
    }
}

