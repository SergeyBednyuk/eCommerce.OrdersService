using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

namespace eCommerce.OrdersService.BLL.Policies;

public static class ResiliencyPolicies
{
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(ILogger logger)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError() // Handles 5xx, 408, and network errors
            .OrResult(msg =>
                msg.StatusCode == System.Net.HttpStatusCode.NotFound)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (result, timeSpan, retryAttempt, context) =>
                {
                    logger.LogWarning(
                        "[Resiliency] Delaying for {{timespan.TotalSeconds}}s, " +
                        "then making retry {{retryAttempt}}. " +
                        "Error: {{outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()}}");
                });
    }

    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(ILogger logger)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (outcome, timespan) =>
                {
                    logger.LogError(
                        $"[Resiliency] Circuit Breaker OPENED for {timespan.TotalSeconds}s " +
                        $"due to: {outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()}");
                },
                onReset: () =>
                {
                    logger.LogInformation("[Resiliency] Circuit Breaker RESET. Requests are flowing again.");
                },
                onHalfOpen: () =>
                {
                    logger.LogInformation("[Resiliency] Circuit Breaker is HALF-OPEN. Testing connection...");
                }
            );
    }
}