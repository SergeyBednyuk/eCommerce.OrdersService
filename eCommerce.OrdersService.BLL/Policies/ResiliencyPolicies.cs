using Microsoft.Extensions.Logging;
using Polly;
using Polly.Bulkhead;
using Polly.CircuitBreaker;
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
                retryCount: 2,
                sleepDurationProvider: retryAttempt => TimeSpan.FromMilliseconds(200 * Math.Pow(2, retryAttempt)),
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
                handledEventsAllowedBeforeBreaking: 3,
                durationOfBreak: TimeSpan.FromSeconds(10),
                onBreak: (outcome, timespan) =>
                {
                    logger.LogError(
                        $"[Resiliency] Circuit Breaker OPENED for {timespan.TotalSeconds}s " +
                        $"due to: {outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()}");
                },
                onReset: () =>
                {
                    logger.LogInformation($"[Resiliency] Circuit Breaker RESET. Requests are flowing again.");
                },
                onHalfOpen: () =>
                {
                    logger.LogInformation($"[Resiliency] Circuit Breaker is HALF-OPEN. Testing connection...");
                }
            );
    }

    public static AsyncPolicy<HttpResponseMessage> GetBulkheadPolicy(ILogger logger, int maxParallelization = 5,
        int maxQueuingActions = 10)
    {
        return Policy.BulkheadAsync<HttpResponseMessage>(
            maxParallelization,
            maxQueuingActions,
            onBulkheadRejectedAsync: context =>
            {
                logger.LogInformation(
                    "Orders API reached {0} max of concurrent actions  or {1} max number of actions that may be queuing",
                    maxParallelization, maxQueuingActions);
                return Task.CompletedTask;
            }
        );
    }

    public static AsyncPolicy<HttpResponseMessage> GetTimeoutPolicy(int seconds = 15)
    {
        return Policy.TimeoutAsync<HttpResponseMessage>(seconds);
    }

    //Optional policy that return default object or check a local Redis cache.
    public static AsyncPolicy<HttpResponseMessage> GetFallbackPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<BrokenCircuitException>()
            .Or<BulkheadRejectedException>()
            .FallbackAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("{\"id\": \"0000\", \"name\": \"Guest User\"}")
            });
    }
}